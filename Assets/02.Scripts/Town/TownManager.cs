using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TownManager : MonoBehaviour
{
    public PlayerController Player => _player;

    [SerializeField] private Transform playerPoint;
    [SerializeField] private PolygonCollider2D borderCollider2D;
    
    [SerializeField] private List<Sprite> cutsceneSlides = new();
    [SerializeField] private float slideDuration = 3.0f;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private bool clickToAdvance = true;
    [SerializeField] private KeyCode advanceKey = KeyCode.Space;
    [SerializeField] private KeyCode skipKey = KeyCode.Escape;

    private PlayerController _player;
    
    
    private void Awake()
    {
        _player =  GameManager.Instance.Player;
        
        CameraManager.Instance.SetConfiner(borderCollider2D);

        _player.ToggleLockSprint(false);
        _player.PlayerInstance.SetSoul(0);
    
        _player.StatHandler.RemoveModifiersFromSource(Constant.TowerSceneName);
        
        var health = _player.StatusHandler.GetStatus(StatType.Health);

        if (health != null)
        {
            _player.StatusHandler.SetStatus(StatType.Health, health.MaxValue);
        }
        
        InitUI();
    }

    
    private void Start()
    {
        GameManager.Instance.DataManager.SavePlayer(_player.PlayerInstance);
        
        _player.MoveDirectTo(playerPoint.position);

        _player.CanDamageable = true;
        
        var cutscene = UIManager.Instance.CreateUI<CutsceneUI>(gameObject.scene);
        
        if (_player != null)
            _player.InputController.ToggleInput(false);
        
        cutscene.Play(
            slides: cutsceneSlides,
            slideDuration: slideDuration,
            fadeSeconds: fadeTime,
            clickToAdvance: clickToAdvance,
            advanceKey: advanceKey,
            skipKey: skipKey,
            onFinished: () =>
            {
                if (_player != null)
                    _player.InputController.ToggleInput(true); 

                AudioManager.Instance.Play("TownBGM"); 
            }
        );
    }


    private void OnDestroy()
    {
        CameraManager.Instance.SetConfiner(null);
        
        AudioManager.Instance.StopLoopAudio("TownBGM");
    }

    private void InitUI()
    {
        var uiManager = UIManager.Instance;
        var itemShopUI = uiManager.CreateUI<TownItemShopUI>(gameObject.scene);
        var teleportUI = uiManager.CreateUI<TownTeleportUI>(gameObject.scene);
        var townHUD = uiManager.CreateUI<TownHUDUI>(gameObject.scene);
        
        itemShopUI.InitPlayer(_player);
        teleportUI.Init(this);
        townHUD.Enable();
    }
}
