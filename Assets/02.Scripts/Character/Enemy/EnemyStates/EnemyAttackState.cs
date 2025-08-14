using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class EnemyAttackState:EnemyBaseState
{
    protected List<EnemyBaseAttackDataSO> attackDataList;

    protected bool isAttack;
    
    protected float attackRate;
    protected float attackDelay;

    protected String attackEffectName;
    
    protected Vector2 attackDirection;
    
    protected WaitForSeconds waitForAfterAttackDelay;

    private bool _isFlipAttackEffectPosition;
    
    public EnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        
        ResetAnimationBool();
        controller.OnAttackStart -= OnStart;
        controller.OnAttackHit -= OnHit;
        controller.OnAttackEnd -= OnEnd;
        
        controller.Anim.speed = 1f;
        
        controller.SuperArmorHandler.RemoveSuperArmor();
    }
    
    public override void Update()
    {
        base.Update();
    }

    protected abstract void OnStart();
    protected abstract void OnHit();
    protected abstract void OnEnd();

    protected IEnumerator ReadyForAttack(float attackDelay, float attackRate, Action callback)
    {
        isAttack = false;
        this.attackDelay = attackDelay;
        this.attackRate = attackRate;
        moveHandler.StopMove();
        
        controller.OnAttackStart += OnStart;
        controller.OnAttackHit += OnHit;
        controller.OnAttackEnd += OnEnd;
        
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, true);
        yield return null;
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, false);

        FlipAttackEffectPosition();
        
        callback?.Invoke();;
    }

    protected abstract void Attack();

    protected virtual void SpawnAttackEffect()
    {
        if (attackEffectName != null)
        {
            VFXHandler attackEffect = ObjectPoolManager.Instance.Spawn<VFXHandler>(attackEffectName);

            
            attackEffect.transform.position = controller.attackEffectPosition.position;
            
            attackEffect.transform.SetParent(controller.transform);
            
            Vector3 scale = attackEffect.transform.localScale;
            
            if (controller.attackEffectPosition.localPosition.x < 0f)
            {
                scale.x = -1f;
            }
            else if (controller.attackEffectPosition.localPosition.x > 0f)
            {
                scale.x = 1f;
            }

            attackEffect.transform.localScale = scale;
            
            attackEffect.Init(attackEffectName);
        }
    }

    protected void FlipAttackEffectPosition()
    {
        Vector3 attackEffectPosition = controller.attackEffectPosition.localPosition;
        
        _isFlipAttackEffectPosition = (controller.LookDir.x < 0 && attackEffectPosition.x > 0f) || (controller.LookDir.x > 0 && attackEffectPosition.x < 0f);

        if (_isFlipAttackEffectPosition)
        {
            attackEffectPosition.x = -attackEffectPosition.x;
            controller.attackEffectPosition.localPosition = attackEffectPosition;
        }
    }
    
    protected IEnumerator GlowOnAttack(float duration, Action callback = null)
    {
        yield return null;
        
        Material material = controller.SpriteRenderer.material;
        
        Sequence sequence =  DOTween.Sequence();
        
        sequence.Append(material.DOFloat(3f, EnemyMaterialKey.HitEffectGlowKey,duration)).SetEase(Ease.InCubic);
        sequence.Join(material.DOFloat(1f, EnemyMaterialKey.HitEffectBlendKey,duration)).SetEase(Ease.InCubic).OnComplete(() =>
        {
            material.SetFloat(EnemyMaterialKey.HitEffectGlowKey, 1f);
            material.SetFloat(EnemyMaterialKey.HitEffectBlendKey, 0f); ;
        
            callback?.Invoke();
        });
        
    }

    protected abstract IEnumerator EndAttackRoutine();

  
}
