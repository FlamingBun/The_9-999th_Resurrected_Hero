using UnityEngine;

public class GoblinBossStateMachine:EnemyStateMachine
{
    private EnemyStates[] _attackStates;
    
    private int _attackPatternCount;
    
    private float _totalRandomRate;
    
    
    public GoblinBossStateMachine(EnemyController enemyController) : base(enemyController)
    {
        statesDict = new()
        {
            { EnemyStates.Idle, new EnemyIdleState(enemyController,this)},
            { EnemyStates.Trace, new GoblinBossTraceState(enemyController,this)},
            { EnemyStates.FirstAttack, new GoblinBossAttackWhirlWindFireState(enemyController, this)},
            { EnemyStates.SecondAttack, new GoblinBossAttackJumpAttackState(enemyController, this)},
            { EnemyStates.ThirdAttack, new GoblinBossAttackStompAttackState(enemyController, this)},
            { EnemyStates.FourthAttack, new GoblinBossAttackWhirlWindMoveState(enemyController, this)},
            { EnemyStates.Hit, new EnemyHitState(enemyController, this)},
            { EnemyStates.Dead, new EnemyDeadState(enemyController, this)},
        };

        SetRandomAttack();
        
        SetNextAttack();
    }

    public void SetNextAttack()
    {
        int index=0;
        
        EnemyStates priviousAttackState = nextAttackState;

        bool isNewAttackPattern = false;

        while (!isNewAttackPattern)
        {
            index=0;
        
            float tempRate = 0f;
            
            float randomValue = Random.Range(0, _totalRandomRate);
            
            for (int i = 0; i < _attackPatternCount; i++)
            {
                tempRate += EnemyController.Data.attackPatternList[i].randomRate;
                if (randomValue <= tempRate)
                {
                    index = i;
                    break;
                }
            }
        
            nextAttackState = _attackStates[index];
            
            if (priviousAttackState != nextAttackState)
            {
                isNewAttackPattern = true;
            }
        }

        EnemyBaseAttackDataSO attackData = EnemyController.Data.attackPatternList[index].attackDatas[0];
        
        attackStartRange = attackData.attackStartRange + Random.Range(0f, attackData.attackStartRangeAdjustment);
    }
    
    public override void ChangeAttackState()
    {
        ChangeEnemyState(nextAttackState);
        SetNextAttack();
    }

    private void SetRandomAttack()
    {
        _totalRandomRate = 0;

        _attackStates = new[]
        {
            EnemyStates.FirstAttack,
            EnemyStates.SecondAttack,
            EnemyStates.ThirdAttack,
            EnemyStates.FourthAttack
        };

        _attackPatternCount = EnemyController.Data.attackPatternList.Count;
        
        for (int i = 0; i < _attackPatternCount; i++)
        {
            _totalRandomRate += EnemyController.Data.attackPatternList[i].randomRate;
        }
    }
}
