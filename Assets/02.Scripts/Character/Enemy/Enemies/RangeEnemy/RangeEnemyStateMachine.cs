using UnityEngine;
public class RangeEnemyStateMachine:EnemyStateMachine
{
    public RangeEnemyStateMachine(EnemyController enemyController) : base(enemyController)
    {
        EnemyBaseAttackDataSO attackData = enemyController.Data.attackPatternList[0].attackDatas[0]; 
        
        attackStartRange = attackData.attackStartRange + Random.Range(0f, attackData.attackStartRangeAdjustment);
    }

    public override void ChangeAttackState()
    {
        ChangeEnemyState(EnemyStates.RangeAttack);
    }
}
