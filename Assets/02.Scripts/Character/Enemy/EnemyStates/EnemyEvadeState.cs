using UnityEngine;

public class EnemyEvadeState:EnemyBaseState
{
    private float _startDelay;

    public EnemyEvadeState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        Evade();
    }

    public override void Update()
    {
        base.Update();

        _startDelay -= Time.deltaTime;
        
        if (_startDelay > 0f) return;
        
        if (stateMachine.CheckArriveDestination())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }
    }

    public override void Exit()
    {
        ResetAnimationBool();
        moveHandler.TargetChange(true);
    }

    private void Evade()
    {
        controller.Anim.SetBool(EnemyAnimationHashes.Move, true);
        
        _startDelay = EnemyConstant.stateDelayTime;
        controller.MoveHandler.Evade();
    }
}