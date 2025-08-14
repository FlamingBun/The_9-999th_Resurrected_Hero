using UnityEngine;

public class FixedEnemyStateMachine:EnemyStateMachine
{
    public FixedEnemyStateMachine(EnemyController enemyController) : base(enemyController)
    {
        statesDict = new()
        {
            { EnemyStates.FixedAttack, new FixedEnemyAttackState(enemyController, this)},
            { EnemyStates.Hit, new FixedEnemyHitState(enemyController, this)},
            { EnemyStates.Dead, new EnemyDeadState(enemyController, this)},
        };

        EnemyBaseAttackDataSO attackData = enemyController.Data.attackPatternList[0].attackDatas[0]; 
        
        attackStartRange = attackData.attackStartRange + Random.Range(0f, attackData.attackStartRangeAdjustment);
    }

    public override void ChangeAttackState()
    {
        ChangeEnemyState(EnemyStates.FixedAttack);
    }
}
