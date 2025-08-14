using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GoblinBossAttackJumpAttackState : EnemyAttackState
{
    private EnemyAreaAttackDataSO _areaAttackDataSO;
    private JumpAttackData _jumpAttackData;

    private SpriteRenderer _shadowRenderer;
    private SpriteRenderer _indicator;

    private JumpAttackCollider _jumpAttackCollider;
    
    private bool _isHitFloor;
    private bool _isFlipable;

    private Vector3 defaultEffectScale;
    private Vector2 defaultShadowSize;
    private Vector2 jumpShadowSize;

    public GoblinBossAttackJumpAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[1].attackDatas;

        _areaAttackDataSO = attackDataList[0] as EnemyAreaAttackDataSO;
        _jumpAttackData = _areaAttackDataSO.jumpAttackData;

        _shadowRenderer = controller.shadow.GetComponent<SpriteRenderer>();
        ObjectPoolManager.Instance.CreatePool(_areaAttackDataSO.impact.GetComponent<VFXHandler>());
        
        waitForAfterAttackDelay = new WaitForSeconds(_areaAttackDataSO.afterAttackDelay);

        _jumpAttackCollider = bossController.jumpAttackDamageCollider;
        
        _jumpAttackCollider.Init(_areaAttackDataSO, controller);
        
        defaultShadowSize = _shadowRenderer.size;
        jumpShadowSize = Vector2.zero;

        _indicator = bossController.InstantiateIndicator(_areaAttackDataSO.indicator);
        _indicator.gameObject.SetActive(false);
        
        defaultEffectScale = Vector3.one;
    }

    public override void Enter()
    {
        base.Enter();

        _isHitFloor = false;
        _isFlipable = true;

        float randomAttackRateAdjustment  = Random.Range(-_areaAttackDataSO.attackRateAdjustment, _areaAttackDataSO.attackRateAdjustment);
        bossController.OnHitFloor += OnHitFloor;

        StartAndTrackCoroutine(ReadyForAttack(_areaAttackDataSO.attackDelay, _areaAttackDataSO.attackRate + randomAttackRateAdjustment , Attack));
    }

    public override void Update()
    {
        if (_isFlipable)
        {
            base.Update();            
        }
    }

    public override void Exit()
    {
        base.Exit();

        bossController.OnHitFloor -= OnHitFloor;

        controller.Anim.SetBool(EnemyAnimationHashes.JumpAttack, false);
        controller.SuperArmorHandler.RemoveSuperArmor();
    }

    protected override void Attack()
    {
        isAttack = true;
        stateMachine.isAttack = true;

        controller.Anim.speed = attackRate;
        controller.Anim.SetBool(EnemyAnimationHashes.JumpAttack, true);
    }

    protected override void OnStart()
    {
        controller.Anim.speed = 0f;

        StartAndTrackCoroutine(GlowOnAttack(attackDelay, () =>
        {
            controller.Anim.speed = attackRate;
            Jump();
        }));
    }

    protected override void OnHit()
    {
        controller.Anim.speed = 0f;
    }

    protected void OnHitFloor()
    {
        _isHitFloor = true;
    }

    protected override void OnEnd()
    {
        StartAndTrackCoroutine(EndAttackRoutine());
    }

    protected override IEnumerator EndAttackRoutine()
    {
        while (isAttack)
        {
            yield return null;
        }
        
        ChangeState();
    }

    private void Jump()
    {
        // 점프 시작 사운드 재생
        AudioManager.Instance.Play("BossJumpAttackStartClip");

        bossController.SpawnJumpStartSmoke();
        
        ResetShadowAndEffectOffsets();
        SetShadowAndEffectParent();

        controller.Collider.enabled = false;

        ApplyScaleOverTime(_shadowRenderer, jumpShadowSize, 0.5f);
        
        _shadowRenderer.DOFade(0f, 0.5f).SetEase(Ease.OutQuart);
        
        SyncScale(bossController.animationEffect.transform);

        moveHandler.AILerp.canMove = false;
        
        moveHandler.MakeMove(controller.Rigid, Vector2.up, _jumpAttackData.jumpForce, 0.5f, () =>
        {
            StartAndTrackCoroutine(ChaseTargetInAirRoutine());
        });
    }

    private IEnumerator ChaseTargetInAirRoutine()
    {
        SetShadowAndEffectParent();
        
        bossController.shadow.transform.DOMove(controller.Player.transform.position, 0.2f);
        bossController.attackEffectPosition.transform.DOMove(controller.Player.transform.position, 0.2f).OnComplete(() =>
        {
            SetShadowAndEffectParent(controller.Player.transform);
            ResetShadowAndEffectOffsets();
        });
        
        yield return new WaitForSeconds(_jumpAttackData.chaseTime);

        SetBossAboveTarget();
        
        StartAndTrackCoroutine(DiveRoutine());
    }

    private IEnumerator DiveRoutine()
    {
        _isFlipable = false;
        bool isLanded = false;
        FlipAttackEffectPosition();
        
        ShowIndicator(_shadowRenderer.transform.position);
        
        yield return new WaitForSeconds(_jumpAttackData.beforeDiveDelay);
        
        controller.Anim.speed = attackRate;
        
        ApplyScaleOverTime(_shadowRenderer, defaultShadowSize, _jumpAttackData.diveDuration);
        
        moveHandler.MakeMove(controller.Rigid, Vector2.down, _jumpAttackData.jumpForce, _jumpAttackData.diveDuration, () =>
        {
            SetShadowAndEffectParent(controller.transform);
            ResetShadowAndEffectOffsets();

            _shadowRenderer.size = defaultShadowSize;
            bossController.animationEffect.transform.localScale = defaultEffectScale;

            controller.Collider.enabled = true;
            moveHandler.AILerp.enabled = false;
            
            isLanded = true;
        });
        
        while (!isLanded && !_isHitFloor)
        {
            yield return null;
        }
        
        bossController.SpawnJumpEndSmoke();
        
        Smash();
            
        // 착지 사운드 재생
        AudioManager.Instance.Play("BossJumpAttackEndClip");
        
        yield return waitForAfterAttackDelay;
        
        moveHandler.AILerp.enabled = true;
        moveHandler.AILerp.canMove = true;
        isAttack = false;
    }

    private void ChangeState()
    {
        stateMachine.isAttack = false;

        if (!stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }
        else
        {
            stateMachine.ChangeAttackState();
        }

    }

    private void Smash()
    {
        _shadowRenderer.DOFade(1, 0f);
        _indicator.gameObject.SetActive(false);
        
        Vector3 offset = _jumpAttackData.areaOffset;
        
        _jumpAttackCollider.Spawn(controller.transform.position + offset);
    }


    private void SetBossAboveTarget()
    {
        SetShadowAndEffectParent();
        
        moveHandler.AILerp.enabled = false;
        
        Vector3 movedPosition = controller.Player.transform.position + (Vector3)(Vector2.up * _jumpAttackData.jumpForce);
        
        bossController.transform.position = movedPosition;
        
        bossController.attackEffectPosition.transform.position = movedPosition - (Vector3)(Vector2.up * _jumpAttackData.jumpForce);
        bossController.shadow.transform.position = movedPosition - (Vector3)(Vector2.up * _jumpAttackData.jumpForce);
    }
    
    private void ShowIndicator(Vector3 indicatorPosition)
    {
        _indicator.transform.position = indicatorPosition;
        _indicator.gameObject.SetActive(true);
        
        Color color = _indicator.color;
        color.a = 0f;

        _indicator.color = color;

        _indicator.DOFade(1f, _jumpAttackData.beforeDiveDelay).SetEase(Ease.OutQuart);
    }
    
    private void ResetShadowAndEffectOffsets()
    {
        bossController.shadow.transform.localPosition = Vector3.zero;
        bossController.animationEffect.transform.localPosition = Vector3.zero;
    }

    private void SetShadowAndEffectParent(Transform parent = null)
    {
        bossController.shadow.transform.parent = parent;
        bossController.animationEffect.transform.parent = parent;
    }

    private void SyncScale(Transform target)
    {
        target.localScale = controller.transform.localScale;
    }

    private void ApplyScaleOverTime(SpriteRenderer target, Vector2 targetSize, float duration)
    {
        DOTween.To(() => target.size, x => target.size = x, targetSize, duration);
    }
}
