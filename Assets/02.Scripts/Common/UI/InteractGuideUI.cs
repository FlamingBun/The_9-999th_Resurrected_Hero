using System;
using DG.Tweening;
using UnityEngine;


public class InteractGuideUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.HUD;
    public override bool IsEnabled => gameObject.activeSelf;


    [SerializeField] private CanvasGroup infoGroup;
    
    private Transform _target;

    private void Update()
    {
        if (_target != null)
        {
            infoGroup.transform.position = Camera.main.WorldToScreenPoint(_target.position);
        }
    }


    public void InitTarget(Transform target)
    {
        _target = target;
    }
    
    public override void Enable()
    {
        gameObject.SetActive(true);
        
        infoGroup.alpha = 0;
        infoGroup.DOKill();
        infoGroup.DOFade(1, 0.1f);
    }

    public override void Disable()
    {
        infoGroup.DOKill();
        infoGroup.DOFade(0, 0.1f).OnComplete(() =>
        {
            _target = null;
            gameObject.SetActive(false);
        });
    }
}
