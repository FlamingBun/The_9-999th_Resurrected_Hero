public class FatOrcController : EnemyController
{
    private EnemyContactDamageCollider damageCollider;
    
    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponentInChildren<EnemyContactDamageCollider>();

        OnDeath += () => superArmorHandler.RemoveSuperArmor();
    }

    public override void Init(EnemyDataSO dataSO,  FloorManager floorManager)
    {
        base.Init(dataSO, floorManager);

        EnemyCollisionAttackDataSO attackData = dataSO.attackPatternList[0].attackDatas[0] as EnemyCollisionAttackDataSO;
        
        damageCollider.Init(this, attackData);
    }

    protected override void SetStateMachine()
    {        
        StateMachine = new FatOrcStateMachine(this);
        enemyStateMachine = StateMachine as FatOrcStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
    
    protected override void EndInitializeEffect()
    {
        base.EndInitializeEffect();
        superArmorHandler.SetSuperArmor();
    }
}
