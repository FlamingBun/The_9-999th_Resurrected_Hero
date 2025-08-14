using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerFloorSelectUIFloorSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TowerCurseSO Curse => _curse;
    
    [SerializeField] private Button iconButton;

    private FloorData _floorData;
    private TowerFloorSelectUIFloorEntry _parentEntry;

    private TowerCurseSO _curse;
    
    private RectTransform _rectTransform;
    private UIDottedLineURP _dottedLine;

    private bool _isActive;


 
    public void Init(TowerFloorSelectUIFloorEntry parentEntry, FloorData floorData, TowerCurseSO curse)
    {
        _rectTransform = GetComponent<RectTransform>();
        _dottedLine = GetComponentInChildren<UIDottedLineURP>();

        _parentEntry = parentEntry;
        
        _floorData = floorData;
        _curse = curse;
        
        iconButton.onClick.AddListener(() =>  parentEntry.OnSelectSlot(this));

        _dottedLine.SetAllDirty();
    }

    public void ToggleActiveState(bool active, TowerFloorSelectUIFloorSlot selectedSlot)
    {
        _isActive = active;
        
        iconButton.enabled = active;
        iconButton.image.sprite = active? _floorData.FloorSelectIcon_Enable : _floorData.FloorSelectIcon_Disable;

        if (_isActive)
        {
            Vector3 localPosition = _rectTransform.InverseTransformPoint(selectedSlot.transform.position);
            _dottedLine.startPoint = localPosition;
        }
    }

    public void ToggleDottedLine(bool enable)
    {
        _dottedLine.enabled = enable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play(key: "StatusUIClip");

        _parentEntry.OnPointerEnterToSlot(this);
        
        if (_isActive)
        {
           iconButton.image.sprite = _floorData.FloorSelectIcon_Hover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _parentEntry.OnPointerExitToSlot();
        
        if (_isActive)
        {
            iconButton.image.sprite = _floorData.FloorSelectIcon_Enable;
        }
    }
}
