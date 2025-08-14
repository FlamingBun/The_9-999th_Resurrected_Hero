using System.Collections;
using UnityEngine;
public class DashEnemyAttackState:EnemyAttackState
{
    private EnemyDashAttackDataSO _dashAttackDataSO;
    private EnemyDashAttackHandler _dashAttackHandler;

    private string _footEffectName;  
    
    private float _currentAttackRate;

    private int _maxDashCount;
    private int _currentTargetDashCount;
    private int _currentAttackCount;
    
    public DashEnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[0].attackDatas;

        _dashAttackHandler = new EnemyDashAttackHandler();
        
        _dashAttackDataSO = attackDataList[0] as EnemyDashAttackDataSO;

        ObjectPoolManager.Instance.CreatePool(_dashAttackDataSO.impact.GetComponent<VFXHandler>());
        ObjectPoolManager.Instance.CreatePool(_dashAttackDataSO.attackEffect.GetComponent<VFXHandler>());
        ObjectPoolManager.Instance.CreatePool(_dashAttackDataSO.dashAttackData.footSmoke.GetComponent<VFXHandler>());
        
        attackEffectName = _dashAttackDataSO.attackEffect.name;
        
        _footEffectName = _dashAttackDataSO.dashAttackData.footSmoke.name;

        _maxDashCount = _dashAttackDataSO.dashAttackData.maxDashCount;
        _currentTargetDashCount = Random.Range(1, _maxDashCount+1);
        
        waitForAfterAttackDelay = new WaitForSeconds(_dashAttackDataSO.afterAttackDelay);
    }


    public override void Enter()
    {
        base.Enter();

        if (_currentAttackCount >= _currentTargetDashCount)
        {
            _currentTargetDashCount = Random.Range(1, _maxDashCount+1);
            _currentAttackCount = 0;
        }

        StartAndTrackCoroutine(ReadyForAttack(_dashAttackDataSO.attackDelay,_dashAttackDataSO.attackRate+ Random.Range(-_dashAttackDataSO.attackRateAdjustment, _dashAttackDataSO.attackRateAdjustment), Attack));
    }
    
    public override void Update()
    {
    }
    
    
    public override void Exit()
    {
        base.Exit();
    }

    
    protected override void Attack()
    {
        isAttack = true;
        stateMachine.isAttack = true;
        
        controller.Anim.speed = attackRate;
        
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, false);
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, true);
    }


    protected override void OnStart()
    {
        FlipSprite();
        FlipAttackEffectPosition();
        attackDirection = moveHandler.GetDirection();
        
        if (_currentAttackCount == 0)
        {
            controller.Anim.speed = 0f;
            
            controller.SuperArmorHandler.TrySuperArmorOnAttack();
            
            StartAndTrackCoroutine(GlowOnAttack(attackDelay, () =>
            {
                controller.Anim.speed = attackRate;
            }));
        }
    }

    protected override void OnHit()
    {
        SpawnAttackEffect();
        StartAndTrackCoroutine(_dashAttackHandler.DashRoutine(controller, attackDirection, _dashAttackDataSO, () => { isAttack = false; }));
    }

    protected override void OnEnd()
    {
        _currentAttackCount++;
        if (_currentAttackCount == _currentTargetDashCount)
        {
            StartAndTrackCoroutine(EndAttackRoutine());
        }
        else
        {
            StartAndTrackCoroutine(DelayAttack());
        }
    }


    protected override void SpawnAttackEffect()
    {
        base.SpawnAttackEffect();

        if (_footEffectName != null)
        {
            VFXHandler footEffect = ObjectPoolManager.Instance.Spawn<VFXHandler>(_footEffectName);
            
            footEffect.transform.position = controller.attackEffectPosition.position;
            
            Vector3 footEffectscale = footEffect.transform.localScale;
            
            if (controller.attackEffectPosition.localPosition.x < 0f)
            {
                footEffectscale.x = -1f;
            }
            else if (controller.attackEffectPosition.localPosition.x > 0f)
            {
                footEffectscale.x = 1f;
            }

            footEffect.transform.localScale = footEffectscale;
            
        
            footEffect.Init(_footEffectName);
        }
    }

    protected override IEnumerator EndAttackRoutine()
    {
        while (isAttack)
        {
            yield return null;
        }
        
        controller.SuperArmorHandler.RemoveSuperArmor();
        
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, false);
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, true);
        controller.Anim.speed = 1f;
        
        yield return waitForAfterAttackDelay;

        ChangeState();
    }

    protected void ChangeState()
    {
        stateMachine.isAttack = false;

        if (!stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }

        if (stateMachine.CheckTargetInAttackRange())
        {
            if (controller.IsEvadable && stateMachine.CheckIsNearTarget())
            {
                stateMachine.ChangeEnemyState(EnemyStates.Evade);
            }
            else
            {
                stateMachine.ChangeEnemyState(EnemyStates.Idle);  
            }
        }
    }
    
    private IEnumerator DelayAttack()
    {
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, false);
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, true);
        yield return null;
        Attack();
    }
}
