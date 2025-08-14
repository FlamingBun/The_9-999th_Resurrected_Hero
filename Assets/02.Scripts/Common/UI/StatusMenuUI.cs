using System;
using DG.Tweening;
using UnityEngine;
using TMPro;


public class StatusMenuUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;

    [SerializeField] private TextMeshProUGUI healthValueText;
    [SerializeField] private TextMeshProUGUI attackDamageValueText;
    [SerializeField] private TextMeshProUGUI attackSpeedValueText;
    [SerializeField] private TextMeshProUGUI attackRangeValueText;
    [SerializeField] private TextMeshProUGUI moveSpeedValueText;

    private PlayerController _player;
    private CanvasGroup _canvasGroup;
    
    private Status _health;
    private Stat _attackDamage;
    private Stat _attackSpeed;
    private Stat _attackRange;
    private Stat _moveSpeed;
    
    public void Init(PlayerController player)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _player = player;
        
        var statHandler = player.StatHandler;
        var statusHandler = player.StatusHandler;
    
        _health = statusHandler.GetStatus(StatType.Health);
        _attackDamage = statHandler.GetStat(StatType.AttackDamage);
        _attackSpeed = statHandler.GetStat(StatType.AttackSpeed);
        _attackRange = statHandler.GetStat(StatType.AttackRange);
        _moveSpeed = statHandler.GetStat(StatType.CriticalChance);
    }
   
    public override void Enable()
    {
        AudioManager.Instance.Play(key: "StatusUIClip");

        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.1f);
        
        gameObject.SetActive(true);

        _player.InputController.EnableTownStatusUIInputs();
        _player.InputController.OnClosePopupUI += Disable;

        UpdateStatusInfo();
    }

    public override void Disable()
    {
        AudioManager.Instance.Play(key: "StatusUIClip");

        _canvasGroup.DOFade(0, 0.1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
       
       _player.InputController.EnablePlayerInputs();
       _player.InputController.OnClosePopupUI -= Disable;
    }

    private void UpdateStatusInfo()
    {
        healthValueText.text = $"{_health.CurValue} / {_health.MaxValue}";
        attackDamageValueText.text = $"{_attackDamage.Value}";
        attackSpeedValueText.text = $"{_attackSpeed.Value}";
        attackRangeValueText.text = $"{_attackRange.Value}";
        moveSpeedValueText.text = $"{_moveSpeed.Value}";
    }
   
}
