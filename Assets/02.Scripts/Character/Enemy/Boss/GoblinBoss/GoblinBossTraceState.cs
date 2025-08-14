using System.Diagnostics;

public class GoblinBossTraceState:EnemyBaseState
{
    private GoblinBossController _bossController;

    private bool isTraceStarted;
    private int num;
    public GoblinBossTraceState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        _bossController = enemyController as GoblinBossController;
    }

    public override void Enter()
    {
        base.Enter();
        isTraceStarted = false;
        Trace();
    }

    public override void Update()
    {
        base.Update();

        if (!isTraceStarted) return;
        
        
        if (stateMachine.CheckArriveDestination() && stateMachine.CheckTargetInAttackRange())
        {
            moveHandler.StopMove();
            controller.Anim.SetBool(EnemyAnimationHashes.MoveLoop, false);
        }
    }

    public override void Exit()
    {
        ResetAnimationBool();

        _bossController.OnMoveStart -= OnMoveStart;
        _bossController.OnMoveEnd -= OnMoveEnd;
        
        controller.Anim.SetBool(EnemyAnimationHashes.MoveLoop, false);
        
        controller.Anim.speed = 1f;
        
        isTraceStarted = false;
    }

    private void Trace()
    {
        _bossController.OnMoveStart += OnMoveStart;
        _bossController.OnMoveEnd += OnMoveEnd;
        
        controller.Anim.SetBool(EnemyAnimationHashes.Move, true);
        moveHandler.FollowPlayer();
        isTraceStarted = true;
    }

    private void OnMoveStart()
    {
        controller.Anim.SetBool(EnemyAnimationHashes.Move, false);
        controller.Anim.SetBool(EnemyAnimationHashes.MoveLoop, true);
    }

    private void OnMoveEnd()
    {
        stateMachine.ChangeAttackState();
    }
}
