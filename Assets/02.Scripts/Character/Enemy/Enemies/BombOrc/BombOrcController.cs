using UnityEngine;

public class BombOrcController : RangeEnemyController
{
    [HideInInspector]public GameObject parentOfIndicator;

    protected override void Awake()
    {
        base.Awake();

        parentOfIndicator = new GameObject("Indicator");
        
        //OnDeath += DestroyIndicator;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        //OnDeath -= DestroyIndicator;
    }

    public override void Init(EnemyDataSO enemyDataSO, FloorManager floorManager)
    {
        base.Init(enemyDataSO, floorManager);
        
        parentOfIndicator.transform.SetParent(floorManager.transform);
    }

    protected override void SetStateMachine()
    {        
        StateMachine = new BombOrcStateMachine(this);
        enemyStateMachine = StateMachine as BombOrcStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
    
    public SpriteRenderer InstantiateIndicator(SpriteRenderer indicator)
    {
        return Instantiate(indicator, parentOfIndicator.transform);
    }

 
}
