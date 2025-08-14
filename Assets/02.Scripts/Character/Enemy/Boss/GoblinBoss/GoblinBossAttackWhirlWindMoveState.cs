using System.Collections;
using UnityEngine;

public class GoblinBossAttackWhirlWindMoveState:GoblinBossAttackWhirlWindState
{
    public GoblinBossAttackWhirlWindMoveState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[3].attackDatas;
        
        dashAttackHandler = new EnemyDashAttackHandler();
        
        dashAttackDataSO = attackDataList[0] as EnemyDashAttackDataSO;
        
        ObjectPoolManager.Instance.CreatePool(dashAttackDataSO.impact.GetComponent<VFXHandler>());
        
        ObjectPoolManager.Instance.CreatePool(dashAttackDataSO.whirlWindData.whirlWindSmoke.GetComponent<VFXHandler>());
    }

    protected override void WhirlWind()
    {
        base.WhirlWind();
        
        StartAndTrackCoroutine(PlayWhirlWindSound());
    }

    private IEnumerator PlayWhirlWindSound()
    {
        float timer = 0.2f;
        while (isAttack)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0f)
            {
                AudioManager.Instance.Play("Boss attack telegraphClip");
                timer = 0.2f;
            }

            yield return null;
        }
    }
}
