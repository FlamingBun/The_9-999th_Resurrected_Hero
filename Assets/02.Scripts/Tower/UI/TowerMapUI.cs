using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TowerMapUI: BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;

    [SerializeField] private TowerMapUIRoomSlot uiRoomSlotPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Sprite lineSprite;
    [SerializeField] private RectTransform lineParent;

    private FloorManager _floorManager;
    private PlayerController _player;
    private TowerMinimapHUDUI _towerMinimapHUDUI;
    private List<TowerMapUIRoomSlot> _createdRoomUIs = new();
    private TowerMapUIRoomSlot _enteredUIRoomSlot;
    private List<Image> _pathLines = new();

    public override void Enable()
    {
        AudioManager.Instance.Play(key: "OpenMapUIClip");
        gameObject.SetActive(true);
        
        _player.InputController.EnableFloorMapUIInputs();
        _player.InputController.OnClosePopupUI += Disable;

        _towerMinimapHUDUI?.HideBorder();
        
        if (_enteredUIRoomSlot != null)
        {
            Canvas.ForceUpdateCanvases();
            
            Vector3 worldPos = _enteredUIRoomSlot.RectTransform.position;
            Vector3 localPosInViewport = scrollRect.viewport.InverseTransformPoint(worldPos);

            Vector3 viewportCenter = scrollRect.viewport.rect.center;

            Vector3 delta = localPosInViewport - viewportCenter;

            Vector2 newAnchoredPos = scrollRect.content.anchoredPosition - new Vector2(delta.x, delta.y);
            scrollRect.content.anchoredPosition = newAnchoredPos;
        }
    }

    public override void Disable()
    {
        _player.InputController.EnablePlayerInputs();
        _player.InputController.OnClosePopupUI -= Disable;
        
        _towerMinimapHUDUI.ShowBorder();

        gameObject.SetActive(false);
    }

    
    public void Init(FloorManager floorManager)
    {
        _floorManager = floorManager;
        _player = floorManager.Player;

        _towerMinimapHUDUI = UIManager.Instance.GetUI<TowerHUDUI>().MinimapHudui;

        foreach (var slots in _createdRoomUIs)
        {
            Destroy(slots.gameObject);
        }

        foreach (var line in _pathLines)
        {
            Destroy(line.gameObject);
        }
        
        _createdRoomUIs.Clear();
        _pathLines.Clear();

        foreach (var room in _floorManager.State.TotalRooms)
        {
            var icon = Instantiate(uiRoomSlotPrefab, scrollRect.content.transform);
            icon.Init(floorManager, room, this);
            icon.gameObject.SetActive(false);
            _createdRoomUIs.Add(icon);
        }

        DrawRoomConnections();
    }


    private void DrawRoomConnections()
    {
        foreach (var roomUI in _createdRoomUIs)
        {
            foreach (var kvp in roomUI.LinkedRoomController.ConnectedRooms)
            {
                var targetRoom = kvp.Value;
                var targetRoomUI = _createdRoomUIs.FirstOrDefault(r => r.LinkedRoomController == targetRoom);

                if (targetRoomUI != null)
                {
                    CreateUILine(roomUI, targetRoomUI);
                }
            }
        }
    }

    private void CreateUILine(TowerMapUIRoomSlot from, TowerMapUIRoomSlot to)
    {
        GameObject lineGO = new GameObject("PathLine", typeof(Image));
        lineGO.transform.SetParent(lineParent, false);
        Image lineImage = lineGO.GetComponent<Image>();
        lineImage.sprite = lineSprite; 

        RectTransform rect = lineGO.GetComponent<RectTransform>();
        Vector2 fromEdge = GetEdgePoint(from, to);
        Vector2 toEdge = GetEdgePoint(to, from);
        Vector2 direction = toEdge - fromEdge;
        rect.sizeDelta = new Vector2(10, 5);
        rect.anchoredPosition = fromEdge + direction / 2f;
        rect.rotation = Quaternion.FromToRotation(Vector3.right, direction);
       

        lineImage.gameObject.SetActive(false);
        
        _pathLines.Add(lineImage);
        to.PathLines.Add(lineImage);
        from.PathLines.Add(lineImage);
    }

 
    public void EnableDiscoveredRoomUI(RoomController discoveredRoomController)
    {
        var newRoomUISlot = _createdRoomUIs.FirstOrDefault(ui => ui.LinkedRoomController == discoveredRoomController);
        
        if (newRoomUISlot == null) return;
        
        newRoomUISlot.gameObject.SetActive(true);
        
        foreach (Image pathLine in newRoomUISlot.PathLines)
        {
            pathLine.gameObject.SetActive(true);
        }
    }

    public void SetEnteredUI(RoomController enteredRoomController)
    {
        _enteredUIRoomSlot?.OnExitedSlot();
        
        _enteredUIRoomSlot = _createdRoomUIs.FirstOrDefault(ui => ui.LinkedRoomController == enteredRoomController);
        
        _enteredUIRoomSlot?.OnEnteredSlot();
    }

    
    private Vector2 GetEdgePoint(TowerMapUIRoomSlot from, TowerMapUIRoomSlot to)
    {
        Vector2 fromCenter = from.RectTransform.anchoredPosition;
        Vector2 toCenter = to.RectTransform.anchoredPosition;
        Vector2 fromSize = from.RectTransform.sizeDelta;
        Vector2 dir = (toCenter - fromCenter).normalized;
        Vector2 halfSize = fromSize * 0.5f;
        Vector2 edge = fromCenter + new Vector2(
            Mathf.Sign(dir.x) * halfSize.x * Mathf.Abs(dir.x),
            Mathf.Sign(dir.y) * halfSize.y * Mathf.Abs(dir.y)
        );
        return edge;
    }
}