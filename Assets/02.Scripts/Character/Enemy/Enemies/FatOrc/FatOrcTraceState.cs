public class FatOrcTraceState : EnemyBaseState
{
    public FatOrcTraceState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }
    
    public override void Enter()
    {
        base.Enter();
        
        controller.Anim.SetBool(EnemyAnimationHashes.Move, true);
        
        moveHandler.FollowPlayer();
    }

    public override void Exit()
    {
        ResetAnimationBool();
    }
}
