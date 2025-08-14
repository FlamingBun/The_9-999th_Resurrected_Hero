using UnityEngine;
using System.Collections;

public class GoblinBossAttackWhirlWindFireState : GoblinBossAttackWhirlWindState
{
    private EnemyRangeAttackHandler _rangeAttackHandler;
    private EnemyRangeAttackDataSO[] _rangeAttackDatas;

    private int _attackPatternCount;

    private int _previousAttackPatternIndex;
    private int _currentAttackPatternIndex;
    
    public GoblinBossAttackWhirlWindFireState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[0].attackDatas;
        
        dashAttackHandler = new EnemyDashAttackHandler();
        
        dashAttackDataSO = attackDataList[0] as EnemyDashAttackDataSO;
        
        ObjectPoolManager.Instance.CreatePool(dashAttackDataSO.impact.GetComponent<VFXHandler>());
        
        ObjectPoolManager.Instance.CreatePool(dashAttackDataSO.whirlWindData.whirlWindSmoke.GetComponent<VFXHandler>());
        
        
        _rangeAttackHandler = new EnemyRangeAttackHandler();

        _attackPatternCount = attackDataList.Count - 1;
        
        _rangeAttackDatas = new EnemyRangeAttackDataSO[_attackPatternCount];
        
        for(int i=0 ;i < _attackPatternCount; i++)
        {
            _rangeAttackDatas[i] = attackDataList[i+1] as EnemyRangeAttackDataSO;
        }

        _currentAttackPatternIndex = Random.Range(0, _attackPatternCount);
        
        ObjectPoolManager.Instance.CreatePool(_rangeAttackDatas[0].impact.GetComponent<VFXHandler>());
        
        ObjectPoolManager.Instance.CreatePool(_rangeAttackDatas[0].rangeAttackData.projectileData.prefab.GetComponent<EnemyProjectile>(), EnemyConstant.projectileDefaultCount * 2);
        
        waitForAfterAttackDelay = new WaitForSeconds(dashAttackDataSO.afterAttackDelay);
    }

    protected override void WhirlWind()
    {
        base.WhirlWind();

        _previousAttackPatternIndex = _currentAttackPatternIndex;

        while (_previousAttackPatternIndex == _currentAttackPatternIndex)
        {
            _currentAttackPatternIndex = Random.Range(0, _attackPatternCount);
        }
        
        StartAndTrackCoroutine(FireRoutine());
    }
    
    private IEnumerator FireRoutine()
    {
        float randomFireInterval = Random.Range(_rangeAttackDatas[_currentAttackPatternIndex].rangeAttackData.minSpawnInterval, _rangeAttackDatas[_currentAttackPatternIndex].rangeAttackData.maxSpawnInterval);

        float rotationSpeed = _rangeAttackDatas[_currentAttackPatternIndex].rangeAttackData.rotationSpeed;

        int i = 0;
        
        while (isAttack)
        {
            i++;
            
            randomFireInterval -=  Time.deltaTime;

            bossController.firePointsAnchor.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            
            if (randomFireInterval <= 0)
            {
                foreach (Transform firePoint in bossController.firePoints)
                {
                    Vector3 fireDirection = firePoint.right;
                    
                    if (i % 2 == 0)
                    {
                        if (_rangeAttackDatas[_currentAttackPatternIndex].rangeAttackData.directionFlipable)
                        {
                            fireDirection = -fireDirection;
                        }

                        AudioManager.Instance.Play("MonsterProjectileClip");
                    }
                    
                    _rangeAttackHandler.Fire(controller, _rangeAttackDatas[_currentAttackPatternIndex], fireDirection, firePoint.position);
                }
                
                randomFireInterval = Random.Range(_rangeAttackDatas[_currentAttackPatternIndex].rangeAttackData.minSpawnInterval, _rangeAttackDatas[_currentAttackPatternIndex].rangeAttackData.maxSpawnInterval);
            }

            yield return null;
        }
    }
}
