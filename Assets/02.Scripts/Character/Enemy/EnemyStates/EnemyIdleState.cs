using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyController enemyController , EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        Idle();
    }

    public override void Update()
    {
        base.Update();
        
        if (stateMachine.CheckTargetInAttackRange())
        {
            if (controller.IsEvadable && stateMachine.CheckIsNearTarget())
            {
                stateMachine.ChangeEnemyState(EnemyStates.Evade);    
            }
            else if(stateMachine.isFollowTarget)
            {
                stateMachine.ChangeAttackState(); 
            }
            else
            {
                stateMachine.ChangeEnemyState(EnemyStates.Trace);
            }
        }

        if (stateMachine.isFollowTarget && !stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }

        if (!stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }
    }
    
    public override void Exit()
    {
        ResetAnimationBool();
    }

    private void Idle()
    {
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, true);
        controller.MoveHandler.StopMove();
    }
}
