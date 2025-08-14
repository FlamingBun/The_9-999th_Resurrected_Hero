public class FixedEnemyController : EnemyController
{
    protected override void SetStateMachine()
    {        
        StateMachine = new FixedEnemyStateMachine(this);
        enemyStateMachine = StateMachine as FixedEnemyStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.FixedAttack);
    }
}
