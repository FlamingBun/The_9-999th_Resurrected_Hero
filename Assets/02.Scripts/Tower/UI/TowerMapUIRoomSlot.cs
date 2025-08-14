using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerMapUIRoomSlot: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform RectTransform => _rectTransform;
    public RoomController LinkedRoomController => _linkedRoomController;

    public List<Image> PathLines => _pathLines;
    
    [SerializeField] private Image backgroundImage;   
    [SerializeField] private Image borderImage;         
    [SerializeField] private Image roomTypeIconImage;
    [SerializeField] private GameObject playerIcon;      
    
    [Space(10f)]
    [SerializeField] private Sprite shopIconSprite;
    [SerializeField] private Sprite transporterIconSprite;
    [SerializeField] private Sprite fountainIconSprite;
    [SerializeField] private Sprite bossIconSprite; 

    [Space(10f)]
    [SerializeField] private Sprite exitedRoomSprite;
    [SerializeField] private Sprite enteredRoomSprite;
    
    private List<Image> _pathLines = new();
    private FloorManager _floorManager;
    private TowerMapUI _towerMapUI;
    private RoomController _linkedRoomController;
    private RectTransform _rectTransform;
    private Button _button;


    void OnDisable()
    {
        if (borderImage.enabled)
        {
            borderImage.enabled = false;
        }
    }
    
    
    public void Init(FloorManager floorManager, RoomController roomController, TowerMapUI towerMapUI)
    {
        _button = GetComponentInChildren<Button>();
        _rectTransform = GetComponent<RectTransform>();

        borderImage.enabled = false;

        _floorManager = floorManager;
        _linkedRoomController = roomController;
        _towerMapUI = towerMapUI;
        
        _rectTransform.anchoredPosition = roomController.RoomBounds.center * 2f;
        _rectTransform.sizeDelta =  roomController.RoomBounds.size * 1.8f;
        
        _button.onClick.RemoveAllListeners(); 
        _button.onClick.AddListener(OnSlotClicked);

        SetRoomIcon(roomController.RoomType);
    }

    public void OnEnteredSlot()
    {
        playerIcon.gameObject.SetActive(true);
        
        backgroundImage.sprite = enteredRoomSprite;
        backgroundImage.raycastTarget = false;
    }

    public void OnExitedSlot()
    {
        playerIcon.gameObject.SetActive(false);
        
        backgroundImage.sprite = exitedRoomSprite;
        backgroundImage.raycastTarget = true;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        borderImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.enabled = false;
    }
    
    private void OnSlotClicked()
    {
        AudioManager.Instance.Play(key: "ClickMapUIClip");
        _floorManager.Player.MoveDirectTo(_linkedRoomController.TPPoint.position);
        
        _floorManager.EnterRoom(_linkedRoomController);
        
        _towerMapUI.Disable();
    }

    
    private void SetRoomIcon(RoomType roomType)
    {
        Sprite roomIcon = roomType switch
        {
            RoomType.Boss => bossIconSprite,
            RoomType.BlessingFountain => fountainIconSprite,
            RoomType.Shop => shopIconSprite,
            RoomType.Transporter => transporterIconSprite,
            _ => null
        };

        if (roomIcon != null)
        {
            roomTypeIconImage.enabled = true;
            roomTypeIconImage.sprite = roomIcon;
            roomTypeIconImage.SetNativeSize();
        }
        else
        {
            roomTypeIconImage.enabled = false;
        }
    }
}
