using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusHUDUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.HUD;
    public override bool IsEnabled => gameObject.activeSelf;


    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text goldText;


    private PlayerController _player;
    
    
    public override void Enable() => gameObject.SetActive(true);
    public override void Disable() => gameObject.SetActive(false);


    private void OnDestroy()
    {
        _player.PlayerInstance.OnGoldChanged -= UpdateGoldText;
    }

    public void Init(PlayerController player)
    {
        _player = player;
        _player.PlayerInstance.OnGoldChanged += UpdateGoldText;

        SetHP();
        UpdateGoldText();
    }
    

    public void SetHP()
    {
        var health = _player.StatusHandler.GetStatus(StatType.Health);

        if (health != null)
        {
            hpBar.fillAmount = health.CurValue / health.MaxValue;   
            hpText.text = $"{(int)health.CurValue} / {(int)health.MaxValue}";
        }
    }

    public void UpdateGoldText()
    {
        goldText.text = _player.PlayerInstance.Gold.ToString();
    }
}