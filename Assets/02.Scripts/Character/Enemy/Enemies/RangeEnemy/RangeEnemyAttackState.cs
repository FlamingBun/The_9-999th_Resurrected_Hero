using System.Collections;
using UnityEngine;
public class RangeEnemyAttackState:EnemyAttackState
{ 
    protected EnemyRangeAttackDataSO rangeAttackDataSO;
    protected EnemyRangeAttackHandler rangeAttackHandler;
    
    
    public RangeEnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[0].attackDatas;

        rangeAttackDataSO = attackDataList[0] as EnemyRangeAttackDataSO;

        rangeAttackHandler = new EnemyRangeAttackHandler();

        ObjectPoolManager.Instance.CreatePool(rangeAttackDataSO.impact.GetComponent<VFXHandler>());

        ObjectPoolManager.Instance.CreatePool(rangeAttackDataSO.attackEffect.GetComponent<VFXHandler>());

        attackEffectName = rangeAttackDataSO.attackEffect.name;
        
        waitForAfterAttackDelay = new WaitForSeconds(rangeAttackDataSO.afterAttackDelay);
    }


    public override void Enter()
    {
        base.Enter();
        
        StartAndTrackCoroutine(ReadyForAttack(rangeAttackDataSO.attackDelay, rangeAttackDataSO.attackRate+ Random.Range(- rangeAttackDataSO.attackRateAdjustment, rangeAttackDataSO.attackRateAdjustment), Attack));
    }
    
    public override void Update()
    {
    }

    public override void Exit()
    {
        base.Exit();
        controller.SuperArmorHandler.RemoveSuperArmor();
    }

    protected override void Attack()
    {
        isAttack = true;
        stateMachine.isAttack = true;

        controller.Anim.speed = attackRate;
        
        controller.Anim.SetBool(EnemyAnimationHashes.Idle,false);
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, true);
    }
    

    protected override void OnStart()
    {
        controller.Anim.speed = 0f;
        attackDirection = moveHandler.GetDirection();
        StartAndTrackCoroutine(GlowOnAttack(attackDelay, () =>
        {
            controller.Anim.speed = attackRate;
        }));
    }

    protected override void OnHit()
    {
        moveHandler.MakeMove(controller.Rigid, attackDirection, rangeAttackDataSO.attackMoveDistance);
        StartAndTrackCoroutine(rangeAttackHandler.FireProjectileRoutine(controller, rangeAttackDataSO, attackDirection, () => { isAttack = false;}));
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

        controller.Anim.SetBool(EnemyAnimationHashes.Idle,true);
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, false);
        
        yield return waitForAfterAttackDelay;
        
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
}
