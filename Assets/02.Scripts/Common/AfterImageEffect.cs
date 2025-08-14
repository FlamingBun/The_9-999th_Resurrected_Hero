using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour, IPoolable
{
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _defaultColor = _spriteRenderer.color;
    }

    public void OnSpawn()
    {
        _spriteRenderer.color = _defaultColor;
    }
    
    public void OnDespawn() {}
    

    public void Init(Transform ownerTransform, SpriteRenderer spriteRenderer, float duration)
    {
        transform.position = ownerTransform.position;
        transform.localScale = ownerTransform.localScale;
        
        _spriteRenderer.sprite = spriteRenderer.sprite;
        _spriteRenderer.flipX = spriteRenderer.flipX;

        var targetColor = _spriteRenderer.color;
        targetColor.a = 0;

        _spriteRenderer.DOColor(targetColor, duration).OnComplete(() =>
        {
            ObjectPoolManager.Instance.Return(gameObject.name, this);
        }).SetEase(Ease.OutCubic);
    }


   
}
