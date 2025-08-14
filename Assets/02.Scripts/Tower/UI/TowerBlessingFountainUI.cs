using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerBlessingFountainUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;

    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollPriceText;
    [SerializeField] private CanvasGroup slotsCanvasGroup;
    
    
    private TowerBlessingFountainUISlot[] _slots;
    private PlayerController _player;
    private BlessingFountain _fountain;
    
    
    public void Init(PlayerController player)
    {
        _player = player;

        _slots = GetComponentsInChildren<TowerBlessingFountainUISlot>();

        foreach (var slot in _slots)
        {
            slot.Init(this);
        }
        
    }

    public void InitBless(BlessingFountain fountain)
    {
        if (_fountain == null)
        {
            _fountain = fountain;
        }

        slotsCanvasGroup.alpha = 0;
        slotsCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutCubic);

        rerollButton.onClick.RemoveAllListeners();
        rerollButton.onClick.AddListener(_fountain.Reroll);

        UpdatePriceText();
        
        foreach (var slot in _slots)
        {
            slot.SetInfo(fountain.GetModifier());
        }
    }

    public void OnSelectBlessSlot(TowerBlessingFountainUISlot slot)
    {
        foreach (var modifier in slot.Modifiers)
        {
            _player.StatHandler.AddModifier(modifier);
        }

        AudioManager.Instance.Play("BuffClip");

        _fountain.PlayBuffParticle();
        
        Disable();
    }

    public override void Enable()
    {
        AudioManager.Instance.Play(key: "BlessingUIClip");
        _player.InputController.ToggleInput(false);
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        _player.InputController.ToggleInput(true);
        gameObject.SetActive(false);
    }

    private void UpdatePriceText()
    {
        rerollPriceText.text = _fountain.CurRerollPrice.ToString();
        rerollPriceText.color = _player.PlayerInstance.Soul >= _fountain.CurRerollPrice ? Color.white : Color.red;
    }

}
