using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
   public event UnityAction<Vector2> OnMove;
   public event UnityAction<Vector2> OnLook; 
   public event UnityAction OnDodge;
   public event UnityAction OnInteract;
   public event UnityAction<float> OnPrimaryHold;
   public event UnityAction<float> OnPrimaryCancel;
   public event UnityAction OnSecondary;
  
   public event UnityAction OnEnableFloorMapUI;
   public event UnityAction OnEnableTownStatusUI;
   public event UnityAction OnEnableSettingUI;
   public event UnityAction OnNextDialogue;
   public event UnityAction OnClosePopupUI;
   public bool IsSprintHold => _isSprintHold;

   private PlayerInputs _inputs;
   private Coroutine _primaryInputCoroutine;
   private CameraManager _cameraManager;
   private float _primaryHoldTime;

   private bool _isOverUI;
   
   private bool _isSprintHold;

   private void Awake()
   {
      _inputs = new PlayerInputs();
   }

   private void Start()
   {
      _cameraManager = CameraManager.Instance;
      
      _inputs.Enable();

      EnablePlayerInputs();
      
      RegisterPlayerActionMaps();
      RegisterDialogueActionMaps();

      _inputs.FloorMapUI.Close.started += OnClosePopupUIInput;
      _inputs.TownItemShopUI.Close.started += OnClosePopupUIInput;
      _inputs.TownStatusUI.Close.started += OnClosePopupUIInput;
      _inputs.SettingUI.Close.started += OnClosePopupUIInput;
   }

   private void OnDestroy()
   {
      _inputs.Disable();
      
      UnregisterPlayerActionMaps();
      UnregisterDialogueActionMaps();
      
      _inputs.FloorMapUI.Close.started -= OnClosePopupUIInput;
      _inputs.TownItemShopUI.Close.started -= OnClosePopupUIInput;
      _inputs.TownStatusUI.Close.started -= OnClosePopupUIInput;
      _inputs.SettingUI.Close.started -= OnClosePopupUIInput;
   }

   private void Update()
   {
      _isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
   }

   private void LateUpdate()
   {
      UpdateLookInput();
   }


   public void ToggleInput(bool enable)
   {
      if (enable)
      {
         _inputs.Enable();
      }
      else
      {
         _inputs.Disable();
      }
   }

   public void ToggleTowerMapInput(bool enable)
   {
      if (enable)
      {
         _inputs.Player.EnableFloorMapUI.Enable();
      }
      else
      {
         _inputs.Player.EnableFloorMapUI.Disable();
      }
   }
   
   public void EnablePlayerInputs()
   {
      DisableAllActionMaps();
      _inputs.Player.Enable();
   }
  
   public void EnableFloorMapUIInputs()
   {
      DisableAllActionMaps();
      _inputs.FloorMapUI.Enable();
   }

   public void EnableDialogueUIInputs()
   {
      DisableAllActionMaps();
      _inputs.DialogueUI.Enable();
   }

   public void EnableTownStatusUIInputs()
   {
      DisableAllActionMaps();
      _inputs.TownStatusUI.Enable();
   }

   public void EnableTownItemShopUIInputs()
   {
      DisableAllActionMaps();
      _inputs.TownItemShopUI.Enable();
   }

   public void EnableSettingUIInputs()
   {
      DisableAllActionMaps();
      _inputs.SettingUI.Enable();
   }

   public void DisablePrimaryCharge()
   {
      if (_primaryInputCoroutine != null)
      {
         StopCoroutine(_primaryInputCoroutine);
      };
      
      _primaryHoldTime = 0;
   }


   private void RegisterPlayerActionMaps()
   {
      _inputs.Player.Move.performed += OnMoveInput;
      _inputs.Player.Move.canceled += OnCancelMoveInput;
      
      _inputs.Player.Dodge.started += OnDodgeInput;
      
      _inputs.Player.Sprint.started += OnSprintInput;
      _inputs.Player.Sprint.canceled += OnSprintInput;
      
      _inputs.Player.Primary.started += OnPrimaryInput;
      _inputs.Player.Primary.canceled += OnPrimaryInput;
      
      _inputs.Player.Secondary.started += OnSecondaryInput;
      _inputs.Player.Secondary.canceled += OnSecondaryInput;
      
      _inputs.Player.EnableFloorMapUI.started += OnEnableFloorMapInput;
      _inputs.Player.EnableTownStatUI.started += OnEnableTownStatusUIInput;
      _inputs.Player.EnableSettingUI.started += OnEnableSettingUIInput;
      
      _inputs.Player.Interact.started += OnInteractInput;
   }

   private void UnregisterPlayerActionMaps()
   {
      _inputs.Player.Move.performed -= OnMoveInput;
      _inputs.Player.Move.canceled -= OnCancelMoveInput;
      
      _inputs.Player.Dodge.started -= OnDodgeInput;
      
      _inputs.Player.Sprint.started -= OnSprintInput;
      _inputs.Player.Sprint.canceled -= OnSprintInput;
      
      _inputs.Player.Primary.started -= OnPrimaryInput;
      _inputs.Player.Primary.canceled -= OnPrimaryInput;
      
      _inputs.Player.Secondary.started -= OnSecondaryInput;
      _inputs.Player.Secondary.canceled -= OnSecondaryInput;
      
      _inputs.Player.EnableFloorMapUI.started -= OnEnableFloorMapInput;
      _inputs.Player.EnableTownStatUI.started -= OnEnableTownStatusUIInput;
      _inputs.Player.EnableSettingUI.started -= OnEnableSettingUIInput;
      
      _inputs.Player.Interact.started -= OnInteractInput;
   }


   private void RegisterDialogueActionMaps()
   {
      _inputs.DialogueUI.Next.started += OnNextDialogueInput;
      _inputs.DialogueUI.Close.started += OnClosePopupUIInput;
   }
   
   private void UnregisterDialogueActionMaps()
   {
      _inputs.DialogueUI.Next.started -= OnNextDialogueInput;
      _inputs.DialogueUI.Close.started -= OnClosePopupUIInput;
   }

   private void DisableAllActionMaps()
   {
      foreach (var actionMap in _inputs.asset.actionMaps)
      {
         actionMap.Disable();
      }
   }

   
   private void UpdateLookInput()
   {
      if(!_inputs.Player.enabled) return;
      
      Vector2 lookInput = _inputs.Player.Look.ReadValue<Vector2>();

      Vector2 screenCenter = _cameraManager.MainCamera.WorldToScreenPoint(transform.position);
      
      OnLook?.Invoke((lookInput - screenCenter).normalized);
   }
   
   
   
   private void OnClosePopupUIInput(InputAction.CallbackContext context) => OnClosePopupUI?.Invoke();
   private void OnNextDialogueInput(InputAction.CallbackContext context) => OnNextDialogue?.Invoke();
   private void OnEnableFloorMapInput(InputAction.CallbackContext context) => OnEnableFloorMapUI?.Invoke();
   private void OnEnableTownStatusUIInput(InputAction.CallbackContext context) => OnEnableTownStatusUI?.Invoke();
   private void OnEnableSettingUIInput(InputAction.CallbackContext context) => OnEnableSettingUI?.Invoke();
   private void OnInteractInput(InputAction.CallbackContext context) => OnInteract?.Invoke();
   private void OnMoveInput(InputAction.CallbackContext context) => OnMove?.Invoke(context.ReadValue<Vector2>());
   private void OnCancelMoveInput(InputAction.CallbackContext context) => OnMove?.Invoke(Vector2.zero);


  
 
   
   private void OnDodgeInput(InputAction.CallbackContext context) => OnDodge?.Invoke();

   private void OnSprintInput(InputAction.CallbackContext context)
   {
      switch (context.phase)
      {
         case InputActionPhase.Started : 
            _isSprintHold = true; 
            break;
         case InputActionPhase.Canceled : 
            _isSprintHold = false; 
            break;
      }
   }
      


   private void OnPrimaryInput(InputAction.CallbackContext context)
   {
      switch (context.phase)
      {
         case InputActionPhase.Started :
          
            
            _primaryInputCoroutine = StartCoroutine(PrimaryCoroutine()); break;
         case InputActionPhase.Canceled :

            if (_primaryHoldTime > 0)
            {
               OnPrimaryCancel?.Invoke(_primaryHoldTime);

               DisablePrimaryCharge();
            }
            break;
      }
   }
   
   private IEnumerator PrimaryCoroutine()
   {
      _primaryHoldTime = 0;
      
      while (true)
      {
         if (_isOverUI)
         {
            _primaryHoldTime = 0;
            yield break;
         }
         
         _primaryHoldTime += Time.deltaTime;
         
         OnPrimaryHold?.Invoke(_primaryHoldTime);

         yield return null;
      }
   }
   
   private void OnSecondaryInput(InputAction.CallbackContext context) => OnSecondary?.Invoke();
   
}
