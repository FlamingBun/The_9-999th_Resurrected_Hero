using System.Collections;
using UnityEngine;
public class FixedEnemyAttackState:EnemyAttackState
{
    private EnemyRangeAttackDataSO _rangeAttackData;
    private EnemyRangeAttackHandler _rangeAttackHandler;
    
    private Vector3[] directions;
    
    public FixedEnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[0].attackDatas;

        _rangeAttackData = attackDataList[0] as EnemyRangeAttackDataSO;

        _rangeAttackHandler = new EnemyRangeAttackHandler();
        
        directions = new[] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };

        ObjectPoolManager.Instance.CreatePool(_rangeAttackData.rangeAttackData.projectileData.prefab.GetComponent<EnemyProjectile>(), EnemyConstant.projectileDefaultCount);
        
        waitForAfterAttackDelay = new WaitForSeconds(_rangeAttackData.afterAttackDelay);
    }


    public override void Enter()
    {
        base.Enter();
        StartAndTrackCoroutine(ReadyForAttack(_rangeAttackData.attackDelay,_rangeAttackData.attackRate+ Random.Range(-_rangeAttackData.attackRateAdjustment, _rangeAttackData.attackRateAdjustment), Attack));
    }
    
    public override void Update()
    {
    }

    public override void Exit()
    {
        base.Exit();
        
        controller.SuperArmorHandler.RemoveSuperArmor();
    }

    protected override void Attack()
    {
        isAttack = true;
        stateMachine.isAttack = true;

        controller.Anim.speed = attackRate;
        
        controller.Anim.SetBool(EnemyAnimationHashes.Idle,false);
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, true);
    }

    protected override void OnStart()
    { 
        controller.Anim.speed = 0f;
        StartAndTrackCoroutine(GlowOnAttack(attackDelay, () =>
        {
            controller.Anim.speed = attackRate;
        }));
    }

    protected override void OnHit()
    {
        StartAndTrackCoroutine(_rangeAttackHandler.CrossFireRoutine(controller, _rangeAttackData, () => { isAttack = false;}));
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

        stateMachine.isAttack = false;
        
        controller.Anim.SetBool(EnemyAnimationHashes.Idle,true);
        controller.Anim.SetBool(EnemyAnimationHashes.Attack, false);
        
        yield return waitForAfterAttackDelay;
        
        stateMachine.ChangeEnemyState(EnemyStates.FixedAttack);
    }
}
