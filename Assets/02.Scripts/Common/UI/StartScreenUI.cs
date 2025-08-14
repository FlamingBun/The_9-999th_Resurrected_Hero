using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.PopupBackground;

    public override bool IsEnabled => gameObject.activeInHierarchy;

    [SerializeField] private Button startButton;

    private PlayerController _player;

    private void Awake()
    {
        startButton.onClick.AddListener(OnClickStart);
    }

    
    public void Init(PlayerController player)
    {
        _player = player;
    }
    
    
    public override void Enable()
    {
        if (_player != null)
        {
            _player.InputController.ToggleInput(false);
        }

        gameObject.SetActive(true);
    }

    
    public override void Disable()
    {
        if (_player != null)
            _player.InputController.ToggleInput(true);
        
        gameObject.SetActive(false);
    }
    
    
    private void OnClickStart()
    {
        AudioManager.Instance.Play(key: "ButtonHoverClip");
        
        UIManager.Instance.GetUI<IntroUI>().Enable();
        
        Disable();
    }
}
