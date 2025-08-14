using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerVisualHandler : MonoBehaviour
{
    [Space(20f)]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private AfterImageEffect afterImageEffectPrefab;
    [SerializeField] private float afterImageEffectInterval;
    [SerializeField] private float afterImageEffectDuration;

    [SerializeField] private VFXHandler moveVFXPrefab;


    private PlayerController _player;
    private Coroutine _afterImageCoroutine;
    private Coroutine _moveVFXCoroutine;
    private ObjectPool<AfterImageEffect> _afterImagePool;
    private ObjectPool<VFXHandler> _moveVFXPrefabPool;
    private SortingGroup _upperArm_R_sortingGroup;
    private SortingGroup _upperArm_L_sortingGroup;

    
    private static readonly int DirYHash = Animator.StringToHash("DirY");
  

    public void Init(PlayerController player)
    {
        _player = player;
        _afterImagePool = ObjectPoolManager.Instance.CreatePool(afterImageEffectPrefab);
        _moveVFXPrefabPool = ObjectPoolManager.Instance.CreatePool(moveVFXPrefab);
    }
    

    public void UpdateModelToLookDir()
    {
        Vector3 playerLocalScale = transform.localScale;
        playerLocalScale.x = _player.LookAxis.x;
        
        _player.Anim.SetFloat(DirYHash,  _player.LookAxis.y);
        
        transform.localScale = playerLocalScale;
    }

    public void ActiveDodgeGhostEffect(float duration)
    {
        Material bodyMat = bodyRenderer.material;
        bodyMat.SetFloat(EnemyMaterialKey.HitEffectBlendKey, 1f);
        bodyMat.DOKill();
        bodyMat.DOFloat(0f, EnemyMaterialKey.HitEffectBlendKey, duration);
    }
    
    public void ActiveAfterImageEffect(float duration)
    {
        if (_afterImageCoroutine != null)
        {
            StopCoroutine(_afterImageCoroutine);
        }

        _afterImageCoroutine = StartCoroutine(AfterImageRoutine(duration));
    }

    public void EnableMoveVFX()
    {
        if (_moveVFXCoroutine != null)
        {
            StopCoroutine(_moveVFXCoroutine);
        }

        _moveVFXCoroutine = StartCoroutine(MoveVFXRoutine());
    }

    public void DisableMoveVFX()
    {
        if (_moveVFXCoroutine != null)
        {
            StopCoroutine(_moveVFXCoroutine);
        }
    }

    IEnumerator MoveVFXRoutine()
    {
        while (true)
        {
            if (_player.MoveSpeed > 0)
            {
                float interval = (1 / (1 + _player.MoveSpeed)) * 0.33f;
                
                yield return new WaitForSeconds(interval);
            }
            
            var vfx = _moveVFXPrefabPool.Spawn();
            
            vfx.Init(moveVFXPrefab.gameObject.name);

            vfx.transform.position = transform.position;

            yield return null;
        }
    }
    
    
    IEnumerator AfterImageRoutine(float duration)
    {
        float timer = 0;
        float intervalTimer = 0;

 
        while (timer < duration)
        {
            float deltaTime = Time.deltaTime;
            
            timer += deltaTime;
            intervalTimer += deltaTime;

            if (intervalTimer >= afterImageEffectInterval)
            {
                var effect = _afterImagePool.Spawn();
                effect.Init(transform, bodyRenderer, afterImageEffectDuration);
                
                intervalTimer = 0;
            }
            
            yield return null;
        }
    }
}
