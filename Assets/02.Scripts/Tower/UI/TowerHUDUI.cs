using System;
using TMPro;
using UnityEngine;

public class TowerHUDUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.HUD;
    public override bool IsEnabled => gameObject.activeInHierarchy;
    public TowerMinimapHUDUI MinimapHudui => _minimapHUDUI;

    public BossHealthBar bossHealthBar;
    
    [SerializeField] private TextMeshProUGUI soulCountText;
    [SerializeField] private TextMeshProUGUI curFloorText;
    
    
    private TowerManager _towerManager;
    private TowerMinimapHUDUI _minimapHUDUI;
    private TowerGemEffectHUDUI _gemEffectHUDUI;
    private PlayerController _player;
    
    
    
    public override void Enable() =>  gameObject.SetActive(true);
    public override void Disable() =>  gameObject.SetActive(false);

    
    
    private void Awake()
    {
        _minimapHUDUI = GetComponentInChildren<TowerMinimapHUDUI>();
        _gemEffectHUDUI = GetComponentInChildren<TowerGemEffectHUDUI>();
    }

    private void OnDestroy()
    {
        _player.PlayerInstance.OnSoulChanged -= UpdateSoulText;
    }


    public void Init(TowerManager towerManager, PlayerController player)
    {
        _player = player;
        _player.PlayerInstance.OnSoulChanged += UpdateSoulText;
        
        _towerManager = towerManager;
        
        UpdateFloorText();
        UpdateSoulText();
    }

    
    
    public void UpdateSoulText()
    {
        soulCountText.text = _player.PlayerInstance.Soul.ToString();
    }

    
    
    public void UpdateFloorText()
    {
        curFloorText.text = (_towerManager.CurFloorIndex + 1) + " F";
    }
    

    public void AddGemEffect(Sprite icon) => _gemEffectHUDUI.AddEffectImage(icon);
    public void ClearGemEffect() => _gemEffectHUDUI.ClearEffects();
    
    
    
    public void InitMinimap(FloorManager floorManager) => _minimapHUDUI.Init(floorManager);
    public void UpdateMinimap(RoomController clampRoom) => _minimapHUDUI.UpdateMinimap(clampRoom.RoomBounds);
}
