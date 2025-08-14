public class BombOrcStateMachine:RangeEnemyStateMachine
{
    public BombOrcStateMachine(EnemyController enemyController) : base(enemyController)
    {
        statesDict = new()
        {
            { EnemyStates.Idle, new EnemyIdleState(enemyController,this)},
            { EnemyStates.Trace, new EnemyTraceState(enemyController,this)},
            { EnemyStates.Evade, new EnemyEvadeState(enemyController, this)},
            { EnemyStates.RangeAttack, new BombOrcAttackState(enemyController, this)},
            { EnemyStates.Hit, new EnemyHitState(enemyController, this)},
            { EnemyStates.Dead, new EnemyDeadState(enemyController, this)},
        };
    }
}
