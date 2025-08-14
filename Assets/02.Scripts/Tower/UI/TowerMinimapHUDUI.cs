using System;
using UnityEngine;
using UnityEngine.UI;

public class TowerMinimapHUDUI : MonoBehaviour
{
    [SerializeField] private RawImage _minimapRawImage;
    [SerializeField] private Vector2 mapMin = new Vector2(-30, -30); 
    [SerializeField] private Vector2 mapMax = new Vector2(30, 30);
    [SerializeField] private Image _borderImage;
    
    private FloorMinimapCamera _floorMinimapCamera;
    private RectTransform _miniMapRectTransform;
    private Vector2 _worldMin;
    private Vector2 _worldMax;
    
    
    public void ShowBorder()
    {
        if (_borderImage != null) _borderImage.enabled = true;
    }

    public void HideBorder()
    {
        if (_borderImage != null) _borderImage.enabled = false;
    }

    private void Start()
    {
        ShowBorder(); // 게임 시작 시 기본적으로 테두리 켜기
    }
    
    public void Init(FloorManager floorManager)
    {
        _floorMinimapCamera = floorManager.FloorMinimapCamera;
        _floorMinimapCamera.Init(floorManager.Player);
    }
    
    

    public void UpdateMinimap(Bounds roomBounds)
    {
        _floorMinimapCamera.SetClampBounds(roomBounds.min, roomBounds.max);
        _floorMinimapCamera.MoveTo(roomBounds.center);
    }

   
    private void AdjustMiniMapSize(Vector3Int tilemapSize)
    {
        float scaleFactor = 10f;
        _miniMapRectTransform.sizeDelta = new Vector2(tilemapSize.x, tilemapSize.y) * scaleFactor;
    }
    

    private void Update()
    {
        if (_miniMapRectTransform == null) return;
    }
    private Vector2 WorldToMiniMap(Vector3 worldPos)
    {
        float x = (worldPos.x - _worldMin.x) / (_worldMax.x - _worldMin.x) * _miniMapRectTransform.sizeDelta.x;
        float y = (worldPos.y - _worldMin.y) / (_worldMax.y - _worldMin.y) * _miniMapRectTransform.sizeDelta.y;
        return new Vector2(x, y);
    }

}