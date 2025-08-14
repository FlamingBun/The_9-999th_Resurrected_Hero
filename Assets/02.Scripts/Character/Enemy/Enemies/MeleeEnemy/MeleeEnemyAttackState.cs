using System.Collections;
using UnityEngine;
public class MeleeEnemyAttackState:EnemyAttackState
{
    private EnemyMeleeAttackDataSO _meleeAttackData;

    private EnemyMeleeAttackHandler _meleeAttackHandler;
    
    public MeleeEnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[0].attackDatas;

        _meleeAttackHandler = new EnemyMeleeAttackHandler();
        
        _meleeAttackData = attackDataList[0] as EnemyMeleeAttackDataSO;
        
        waitForAfterAttackDelay = new WaitForSeconds(_meleeAttackData.afterAttackDelay);
    }
    
    public override void Enter()
    {
        base.Enter();
        
        StartAndTrackCoroutine(ReadyForAttack(_meleeAttackData.attackDelay, _meleeAttackData.attackRate+ Random.Range(-_meleeAttackData.attackRateAdjustment, _meleeAttackData.attackRateAdjustment), Attack));
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
        StartAndTrackCoroutine(GlowOnAttack(attackDelay, () =>
        {
            controller.Anim.speed = attackRate;
        }));
    }

    protected override void OnHit()
    {
        StartAndTrackCoroutine(_meleeAttackHandler.MeleeAttack(controller, moveHandler.GetDirection(), _meleeAttackData));
    }

    protected override void OnEnd()
    {
        StartAndTrackCoroutine(EndAttackRoutine());
    }

    protected override IEnumerator EndAttackRoutine()
    {
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
