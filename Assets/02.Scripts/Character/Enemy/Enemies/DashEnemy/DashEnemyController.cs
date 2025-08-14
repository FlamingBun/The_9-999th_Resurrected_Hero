public class DashEnemyController : EnemyController
{
    protected override void SetStateMachine()
    {        
        StateMachine = new DashEnemyStateMachine(this);
        enemyStateMachine = StateMachine as DashEnemyStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
    
    public override void AnimationEvent(string key)
    {
        switch (key)
        {
            case EnemyAnimationEventKey.AttackStartKey:
                StartCoroutine(AnimationEndCheckRoutine(CallAttackEndEvent));

                CallAttackStartEvent();
                break;

            case EnemyAnimationEventKey.AttackHitKey:
                CallAttackHitEvent();
                break;

            case EnemyAnimationEventKey.HitEndKey:
                StartCoroutine(AnimationEndCheckRoutine(CallHitEndEvent));
                break;
        }
    }
}
