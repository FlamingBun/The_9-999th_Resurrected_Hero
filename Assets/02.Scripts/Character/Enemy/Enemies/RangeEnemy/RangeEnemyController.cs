public class RangeEnemyController : EnemyController
{
    protected override void SetStateMachine()
    {        
        StateMachine = new RangeEnemyStateMachine(this);
        enemyStateMachine = StateMachine as RangeEnemyStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
}
