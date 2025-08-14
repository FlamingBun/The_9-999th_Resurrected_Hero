using UnityEngine;

public class MeleeEnemyStateMachine:EnemyStateMachine
{
    public MeleeEnemyStateMachine(EnemyController enemyController) : base(enemyController)
    {
        statesDict = new()
        {
            { EnemyStates.Idle, new EnemyIdleState(enemyController,this)},
            { EnemyStates.Trace, new EnemyTraceState(enemyController,this)},
            { EnemyStates.Evade, new EnemyEvadeState(enemyController,this)},
            { EnemyStates.MeleeAttack, new MeleeEnemyAttackState(enemyController, this)},
            { EnemyStates.Hit, new EnemyHitState(enemyController, this)},
            { EnemyStates.Dead, new EnemyDeadState(enemyController, this)},
        };

        EnemyBaseAttackDataSO attackData = enemyController.Data.attackPatternList[0].attackDatas[0]; 
        
        attackStartRange = attackData.attackStartRange + Random.Range(0f, attackData.attackStartRangeAdjustment);
    }

    public override void ChangeAttackState()
    {
        ChangeEnemyState(EnemyStates.MeleeAttack);
    }
}
