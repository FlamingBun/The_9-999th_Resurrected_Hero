using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class FadeScreenUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Overlay;
    public override bool IsEnabled => _canvasGroup.alpha >= 1f;


    private CanvasGroup _canvasGroup;
    private UnityAction _onComplete;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    public override void Enable()
    {
        gameObject.SetActive(true);
        
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0;
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
        
        _canvasGroup.blocksRaycasts = false;
    }

    public void FadeIn(float duration, UnityAction onComplete)
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = true;
        
        _canvasGroup.DOFade(1, duration).
            SetEase(Ease.InQuad).
            SetUpdate(true).OnComplete(() => onComplete?.Invoke());
    }


    public void FadeOut(float duration, UnityAction onComplete)
    {
        _canvasGroup.DOFade(0, duration).
            SetEase(Ease.InQuad).
            OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = false;
                onComplete?.Invoke();
            }).
            SetUpdate(true);
    }
    
    
}
