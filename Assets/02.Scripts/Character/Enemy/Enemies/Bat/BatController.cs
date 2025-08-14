public class BatController : RangeEnemyController
{
    protected override void SetStateMachine()
    {        
        StateMachine = new BatStateMachine(this);
        enemyStateMachine = StateMachine as BatStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
}
