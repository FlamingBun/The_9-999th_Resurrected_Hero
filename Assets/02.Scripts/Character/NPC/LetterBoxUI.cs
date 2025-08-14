using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class LetterBoxUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.PopupBackground;
    public override bool IsEnabled => gameObject.activeInHierarchy;

    private Image[] _borders;

    private PlayerController _player;

    private void Awake()
    {
        _borders = GetComponentsInChildren<Image>();
    }

    public void Init(PlayerController player)
    {
        _player = player;
        
        foreach (var border in _borders)
        {
            border.fillAmount = 0;
        }
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
        
        foreach (var border in _borders)
        {
            border.DOKill();

            border.DOFillAmount(1, 0.75f).SetEase(Ease.OutQuad);
        }
    }

    public override void Disable()
    {
        foreach (var border in _borders)
        {
            border.DOKill();
            
            border.DOFillAmount(0, 0.75f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
