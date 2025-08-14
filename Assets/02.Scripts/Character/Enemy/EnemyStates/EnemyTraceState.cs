public class EnemyTraceState:EnemyBaseState
{
    public EnemyTraceState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        controller.Anim.SetBool(EnemyAnimationHashes.Move, true);
        
        moveHandler.FollowPlayer();
    }

    public override void Update()
    {
        base.Update();

        if (controller.IsEvadable && stateMachine.CheckIsNearTarget())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Evade);    
        }
        
        if (stateMachine.CheckArriveDestination() && stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeAttackState();
        }
    }

    public override void Exit()
    {
        ResetAnimationBool();
    }
}
