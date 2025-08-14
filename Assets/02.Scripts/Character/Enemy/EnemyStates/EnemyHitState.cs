using UnityEngine;
public class EnemyHitState:EnemyBaseState
{
    private AnimatorStateInfo _stateInfo;
    
    public EnemyHitState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Hit();
    }
    
    public override void Update()
    {
    }
    
    public override void Exit()
    {
        ResetAnimationBool();
        controller.OnHitEnd -= OnHitEnd;
    }

    private void Hit()
    {
        controller.OnHitEnd += OnHitEnd;
        
        controller.MoveHandler.StopMove();
        controller.Anim.SetBool(EnemyAnimationHashes.Hit, true);
    }

    private void OnHitEnd()
    {
        // stateMachine.ChangeEnemyState(EnemyStates.Trace);
        stateMachine.ChangeAttackState();
    }
}
