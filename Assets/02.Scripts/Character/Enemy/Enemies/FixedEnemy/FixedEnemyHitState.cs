using UnityEngine;
public class FixedEnemyHitState:EnemyHitState
{
    private AnimatorStateInfo _stateInfo;
    
    public FixedEnemyHitState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }
    
    public override void Update()
    {
        _stateInfo = controller.Anim.GetCurrentAnimatorStateInfo(0);
        
        if (_stateInfo.IsName("Hit"))
        {
            if (_stateInfo.normalizedTime < 1.0f)
            {
                _stateInfo = controller.Anim.GetCurrentAnimatorStateInfo(0);
            }
            else
            {
                stateMachine.ChangeEnemyState(EnemyStates.FixedAttack);
            }
        }
    }
    
}
