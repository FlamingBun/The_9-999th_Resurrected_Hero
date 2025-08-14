using UnityEngine;
using System.Collections;

public class GoblinBossAttackWhirlWindState:EnemyAttackState
{
    protected EnemyDashAttackHandler dashAttackHandler;
    

    protected EnemyDashAttackDataSO dashAttackDataSO;
    
    
    public GoblinBossAttackWhirlWindState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
    }


    public override void Enter()
    {
        base.Enter();

        float randomAttackRateAdjustment = Random.Range(-dashAttackDataSO.attackRateAdjustment, dashAttackDataSO.attackRateAdjustment);
        
        StartAndTrackCoroutine(ReadyForAttack(dashAttackDataSO.attackDelay, dashAttackDataSO.attackRate + randomAttackRateAdjustment, Attack));
    }

    public override void Exit()
    {
        base.Exit();
        
        controller.Anim.SetBool(EnemyAnimationHashes.WhirlWind, false);
        controller.Anim.SetBool(EnemyAnimationHashes.WhirlWindLoop, false);
        
        controller.SuperArmorHandler.RemoveSuperArmor();
    }

    protected override void Attack()
    {
        isAttack = true;
        stateMachine.isAttack = true;
        
        controller.Anim.SetBool(EnemyAnimationHashes.WhirlWind, true);
    }

    protected override void OnStart()
    {
        controller.Anim.speed = 0f;

        // 사전 경고음 재생 추가
        AudioManager.Instance.Play("Boss attack telegraphClip");

        StartAndTrackCoroutine(GlowOnAttack(attackDelay, () =>
        {
            controller.Anim.speed = attackRate;
            WhirlWind();
        }));
    }

    protected override void OnHit()
    {
    }

    protected override void OnEnd()
    {
        StartAndTrackCoroutine(EndAttackRoutine());
    }

    protected override IEnumerator EndAttackRoutine()
    {
        while (isAttack)
        {
            yield return null;
        }
        
        yield return waitForAfterAttackDelay;
        
        ChangeState();
    }

    protected virtual void WhirlWind()
    {
        controller.Anim.SetBool(EnemyAnimationHashes.WhirlWind, false);
        controller.Anim.SetBool(EnemyAnimationHashes.WhirlWindLoop, true);
        StartAndTrackCoroutine(dashAttackHandler.WhirlWindRoutine(controller, dashAttackDataSO, () => { isAttack = false; controller.Anim.SetBool(EnemyAnimationHashes.WhirlWindLoop, false); }));
    }

   

    private void ChangeState()
    {
        stateMachine.isAttack = false;

        if (!stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }
        else
        {
            stateMachine.ChangeAttackState();
        }
    }
}
