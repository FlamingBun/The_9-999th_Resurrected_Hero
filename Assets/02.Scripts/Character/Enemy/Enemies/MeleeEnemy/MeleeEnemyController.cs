public class MeleeEnemyController : EnemyController
{
    protected override void SetStateMachine()
    {        
        StateMachine = new MeleeEnemyStateMachine(this);
        enemyStateMachine = StateMachine as MeleeEnemyStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
}
