public class BatAttackState:RangeEnemyAttackState
{ 
    public BatAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        ObjectPoolManager.Instance.CreatePool(rangeAttackDataSO.rangeAttackData.projectileData.prefab.GetComponent<EnemyProjectile>(), EnemyConstant.projectileDefaultCount);
    }

    protected override void OnHit()
    {
        SpawnAttackEffect();
        
        moveHandler.MakeMove(controller.Rigid, attackDirection, rangeAttackDataSO.attackMoveDistance);
        StartAndTrackCoroutine(rangeAttackHandler.FireProjectileRoutine(controller, rangeAttackDataSO, attackDirection, () => { isAttack = false;}));
    }
}
