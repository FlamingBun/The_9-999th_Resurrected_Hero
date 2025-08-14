using UnityEngine;

public class FatOrcStateMachine:EnemyStateMachine
{
    public FatOrcStateMachine(EnemyController enemyController) : base(enemyController)
    {
        statesDict = new()
        {
            { EnemyStates.Trace, new FatOrcTraceState(enemyController,this)},
            { EnemyStates.Hit, new EnemyHitState(enemyController, this)},
            { EnemyStates.Dead, new EnemyDeadState(enemyController, this)},
        };
    }

    public override void ChangeAttackState()
    {
    }
}
