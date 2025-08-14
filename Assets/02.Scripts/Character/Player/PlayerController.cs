using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class PlayerController : CharacterBaseController
{
    public PlayerInstance PlayerInstance => _playerInstance;
    public PlayerInputController InputController => _inputController;
    public PlayerEquipmentHandler EquipmentHandler => _equipmentHandler;
    public PlayerWeaponHandler WeaponHandler => _weaponHandler;
    public PlayerVisualHandler VisualHandler => _visualHandler;
    public PlayerInteractionHandler InteractionHandler => _interactionHandler;
    public PlayerParryTriggerHandler ParryTriggerHandler => parryTriggerHandler;
    public Vector2Int LookAxis => new(LookDir.x > 0 ? 1 : -1, LookDir.y > 0 ? 1 : -1);
    public float MoveSpeed => _moveSpeed;
    public bool IsLockedSprint => _isLockedSprint;


    [SerializeField] private TriggerHandler moveBackTriggerHandler;
    [SerializeField] private TriggerHandler interactTriggerHandler;
    [SerializeField] private PlayerParryTriggerHandler parryTriggerHandler;


    private CameraManager _cameraManager;
    private PlayerInstance _playerInstance;
    private PlayerInteractionHandler _interactionHandler;
    private PlayerEquipmentHandler _equipmentHandler;
    private PlayerWeaponHandler _weaponHandler;
    private PlayerInputController _inputController;
    private PlayerVisualHandler _visualHandler;
    private Tweener _attackDashTweener;

    private StatusMenuUI _statusMenuUI;
    private StatusHUDUI _statusHUDUI;
    private SettingUI _settingUI;

    private float _moveSpeed;
    private bool _isLockedSprint;
    private bool _isParry;
    private bool _isClampBounds;

    private Bounds _clampBounds;


    protected override void Awake()
    {
        base.Awake();

        CanDamageable = true;

        _weaponHandler = GetComponent<PlayerWeaponHandler>();
        _equipmentHandler = GetComponent<PlayerEquipmentHandler>();
        _visualHandler = GetComponent<PlayerVisualHandler>();
        _inputController = GetComponent<PlayerInputController>();
        _interactionHandler = GetComponent<PlayerInteractionHandler>();

        StateMachine = new PlayerStateMachine(this);

        if (StateMachine is PlayerStateMachine playerStateMachine)
        {
            playerStateMachine.ChangeIdleState();
        }
    }

    // TESTCODE
    // private void LateUpdate()
    // {
    //     if (Input.GetKeyDown(KeyCode.Q))
    //     {
    //         PlayerInstance.ModifySoul(100);
    //         PlayerInstance.ModifyGold(100);
    //     }
    // }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        var targetMovePos = Rigid.position + MoveDir * _moveSpeed;

        if (StateMachine.CurrentState is PlayerMoveState)
        {
            Rigid.MovePosition(targetMovePos);
        }

        if (_isClampBounds)
        {
            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(targetMovePos.x, _clampBounds.min.x, _clampBounds.max.x),
                Mathf.Clamp(targetMovePos.y, _clampBounds.min.y, _clampBounds.max.y)
            );

            if (targetMovePos != clampedPos)
            {
                Rigid.DOKill();
                Rigid.MovePosition(clampedPos);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        InputController.OnEnableTownStatusUI -= _statusMenuUI.Enable;
        InputController.OnEnableSettingUI -= _settingUI.Enable;

        moveBackTriggerHandler.OnStay -= UpdateMoveBack;
        StatusHandler.OnStatusChanged -= UpdateHealthBar;
        StatusHandler.OnStatusChanged -= _cameraManager.PlayTakeDamageImpulse;
        StatusHandler.OnStatusChanged -= PlayHitSFx;

        OnDeath -= Death;
    }


    public void Init(PlayerInstance playerInstance)
    {
        _cameraManager = CameraManager.Instance;
        _playerInstance = playerInstance;

        _cameraManager.SetTargetToFollowCamera(transform);

        StatHandler.Init(playerInstance.defaultStatDatas);
        StatusHandler.Init(StatHandler);

        _interactionHandler.Init(this, interactTriggerHandler);
        _equipmentHandler.Init(this);
        _weaponHandler.Init(this);
        _visualHandler.Init(this);
        parryTriggerHandler.Init(this);

      

        var uiManager = UIManager.Instance;

        _statusMenuUI = uiManager.GetUI<StatusMenuUI>();
        _statusHUDUI = uiManager.GetUI<StatusHUDUI>();
        _settingUI = uiManager.GetUI<SettingUI>();

        _statusMenuUI.Init(this);
        _settingUI.Init(this);



        InputController.OnEnableTownStatusUI += _statusMenuUI.Enable;
        InputController.OnEnableSettingUI += _settingUI.Enable;

        moveBackTriggerHandler.OnStay += UpdateMoveBack;
        StatusHandler.OnStatusChanged += UpdateHealthBar;
        StatusHandler.OnStatusChanged += _cameraManager.PlayTakeDamageImpulse;
        StatusHandler.OnStatusChanged += PlayHitSFx;

        OnDeath += Death;
    }

    public void ToggleClampBounds(bool clamp, Bounds bounds)
    {
        _isClampBounds = clamp;
        _clampBounds = bounds;
    }


    public void AttackDash(Vector2 dashDir, float dashDist)
    {
        Rigid.DOKill();
        float dashDuration = 0.5f;
        _attackDashTweener = Rigid.DOMove(Rigid.position + dashDir * dashDist, dashDuration).SetEase(Ease.OutCubic);
    }

    public void UpdateLookDir(Vector2 inputDir)
    {
        LookDir = inputDir;
    }

    public void UpdateMoveDir(Vector2 inputDir) => MoveDir = inputDir;


    public void SetMove(float adjustRate = 1)
    {
        _moveSpeed = StatHandler.GetStat(StatType.MoveSpeed).Value * adjustRate * Time.fixedDeltaTime;
    }


    public void ToggleLockSprint(bool enable) => _isLockedSprint = enable;


    public void MoveDirectTo(Vector3 pos)
    {
        Rigid.DOKill();

        var delta = pos - transform.position;

        CameraManager.Instance.Teleport(transform, delta);

        transform.position = pos;
    }


    private void UpdateMoveBack(Collider2D other)
    {
        if (other.TryGetComponent(out CharacterBaseController otherCharacter))
        {
            if (!IsLockedMoveBack)
            {
                _attackDashTweener.Kill();

                MoveBack(otherCharacter);
            }

            if (!otherCharacter.IsLockedMoveBack) otherCharacter.MoveBack(this);
        }
    }

    private void PlayHitSFx(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            if (eventData.CurValue < eventData.PreValue)
            {
                AudioManager.Instance.Play("PlayerHitClip", 0.5f);
            }
        }
    }


    private void Death()
    {
        GameManager.Instance.Death();
        CanDamageable = false;
    }


    private void UpdateHealthBar(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            _statusHUDUI.SetHP();
        }
    }
}
