using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyRangeAttackHandler
{
    public void Fire(EnemyController controller, EnemyRangeAttackDataSO enemyRangeAttackData, Vector2 fireDirection, Vector2 projectileSpawnPosition)
    {
        EnemyRangeAttackDataSO rangeAttackDataSO = enemyRangeAttackData;
        EnemyRangeAttackData rangeAttackData = rangeAttackDataSO.rangeAttackData;
        
        EnemyProjectile projectile = ObjectPoolManager.Instance.Spawn<EnemyProjectile>(rangeAttackData.projectileData.prefab.name);

        projectile.transform.position = projectileSpawnPosition;

        projectile.Init(controller, rangeAttackDataSO, rangeAttackData.projectileData, fireDirection);
        
    }

    public IEnumerator FireProjectileRoutine(EnemyController controller, EnemyRangeAttackDataSO enemyRangeAttackData, Vector2 fireDirection, Action FireCallback = null)
    {
        EnemyRangeAttackDataSO rangeAttackDataSO = enemyRangeAttackData;
        EnemyRangeAttackData rangeAttackData = rangeAttackDataSO.rangeAttackData;

        Vector2 direction;
        
        int spreadToggle;
        float randomSpread;

        float baseAngle;
        float finalAngle;

        float rad;
        
        Vector2 spreadDirection;

        for (int i = 0; i < rangeAttackData.fireCount; i++)
        {

            for (int j = 0; j < rangeAttackData.spawnCount; j++)
            {
                spreadToggle = Random.Range(0, 2) * 2 - 1;

                randomSpread = spreadToggle + Random.Range(rangeAttackData.minSpread, rangeAttackData.maxSpread);

                if (fireDirection !=  Vector2.zero)
                {
                    direction = fireDirection.normalized;
                }
                else
                {
                    direction = controller.MoveHandler.GetDirection();   
                }
                
                baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                finalAngle = baseAngle + randomSpread;
                
                rad = finalAngle * Mathf.Deg2Rad;
                spreadDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
                
                EnemyProjectile projectile = ObjectPoolManager.Instance.Spawn<EnemyProjectile>(rangeAttackData.projectileData.prefab.name);

                projectile.transform.position = (controller.transform.position + (Vector3)rangeAttackData.projectileSpawnOffset) + (Vector3)(spreadDirection * rangeAttackData.directionalOffset);
                
                projectile.transform.eulerAngles = new Vector3(0, 0, finalAngle);

                projectile.Init(controller, rangeAttackDataSO, rangeAttackData.projectileData, spreadDirection);
                AudioManager.Instance.Play("MonsterProjectileClip");
                
                yield return new WaitForSeconds(Random.Range(rangeAttackData.minSpawnInterval, rangeAttackData.maxSpawnInterval));
            }

            yield return new WaitForSeconds(Random.Range(rangeAttackData.minFireInterval, rangeAttackData.maxFireInterval));
        }

        FireCallback?.Invoke();
    }

    public IEnumerator FireBombRoutine(EnemyController controller, EnemyRangeAttackDataSO enemyRangeAttackData, Vector2 fireDirection, Action<Vector3> IndicatorCallback, Action FireCallback = null, GameObject indicatorObj = null)
    {
        EnemyRangeAttackDataSO rangeAttackDataSO = enemyRangeAttackData;
        EnemyRangeAttackData rangeAttackData = rangeAttackDataSO.rangeAttackData;

        EnemyProjectileData projectileData = rangeAttackData.projectileData;
        
        Vector2 direction;
        
        int spreadToggle;
        float randomSpread;

        float baseAngle;
        float finalAngle;

        float rad;
        
        Vector2 spreadDirection;


        for (int i = 0; i < rangeAttackData.fireCount; i++)
        {

            for (int j = 0; j < rangeAttackData.spawnCount; j++)
            {
                spreadToggle = Random.Range(0, 2) * 2 - 1;

                randomSpread = spreadToggle + Random.Range(rangeAttackData.minSpread, rangeAttackData.maxSpread);

                if (fireDirection !=  Vector2.zero)
                {
                    direction = fireDirection.normalized;
                }
                else
                {
                    direction = controller.MoveHandler.GetDirection();   
                }
                
                baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                finalAngle = baseAngle + randomSpread;
                
                rad = finalAngle * Mathf.Deg2Rad;
                spreadDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
                
                EnemyBomb bomb = ObjectPoolManager.Instance.Spawn<EnemyBomb>(projectileData.prefab.name);

                bomb.transform.position = (controller.transform.position + (Vector3)rangeAttackData.projectileSpawnOffset) + (Vector3)(spreadDirection * rangeAttackData.directionalOffset);

                bomb.Init(controller, rangeAttackDataSO, projectileData, spreadDirection);
                bomb.indicatorObj = indicatorObj;

                float moveRange = Mathf.Min(controller.MoveHandler.GetDistanceFromPlayer(), Random.Range(projectileData.minRange,projectileData.maxRange));
        
                Vector2 targetPosition = bomb.transform.position + ((Vector3)spreadDirection * moveRange);
                
                IndicatorCallback?.Invoke(targetPosition);
                
                bomb.ParabolaMove(bomb.transform, bomb.transform.position, targetPosition, rangeAttackData.bombArcPeakHeight, rangeAttackData.bombFlightTime);
                
                yield return new WaitForSeconds(Random.Range(rangeAttackData.minSpawnInterval, rangeAttackData.maxSpawnInterval));
            }

            yield return new WaitForSeconds(Random.Range(rangeAttackData.minFireInterval, rangeAttackData.maxFireInterval));
        }

        FireCallback?.Invoke();
    }


    public IEnumerator CrossFireRoutine(EnemyController controller, EnemyRangeAttackDataSO enemyRangeAttackData, Action FireCallback = null)
    {
        EnemyRangeAttackDataSO rangeAttackDataSO = enemyRangeAttackData;
        EnemyRangeAttackData rangeAttackData = rangeAttackDataSO.rangeAttackData;
        
        int spawnCount = rangeAttackData.spawnCount;
        
        float[] angles = new float[] { 0f, 90f, 180f, 270f };
        float angleRandomRange = rangeAttackData.randomAngleRange;
        
        for (int i = 0; i < rangeAttackData.fireCount; i++)
        {
            for (int j = 0; j < spawnCount; j++)
            {
                float randomOffset = Random.Range(0, angleRandomRange);
                float angle = angles[j] + randomOffset;
                
                float rad = angle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                EnemyProjectile projectile = ObjectPoolManager.Instance.Spawn<EnemyProjectile>(rangeAttackData.projectileData.prefab.name);
                projectile.transform.position = (controller.transform.position+(Vector3)rangeAttackData.projectileSpawnOffset) + (Vector3)(direction *rangeAttackData.directionalOffset);

                projectile.transform.eulerAngles = new Vector3(0, 0, angle);

                projectile.Init(controller, rangeAttackDataSO, rangeAttackData.projectileData, direction);
                AudioManager.Instance.Play("MonsterProjectileClip");
                
                yield return new WaitForSeconds(Random.Range(rangeAttackData.minSpawnInterval, rangeAttackData.maxSpawnInterval));
            }
            yield return new WaitForSeconds(Random.Range(rangeAttackData.minFireInterval, rangeAttackData.maxFireInterval));
        }
        FireCallback?.Invoke();
    }
    
    public IEnumerator RoundFireRoutine(EnemyController controller, EnemyRangeAttackDataSO enemyRangeAttackData, Action FireCallback = null)
    {
        EnemyRangeAttackDataSO rangeAttackDataSO = enemyRangeAttackData;
        EnemyRangeAttackData rangeAttackData = rangeAttackDataSO.rangeAttackData;
        
        int spawnCount = rangeAttackData.spawnCount;
        
        for (int i = 0; i < rangeAttackData.fireCount; i++)
        {
            for (int j = 0; j < spawnCount; j++)
            {
                Vector2 direction = new Vector2(Mathf.Cos(Mathf.PI * 2 * j /spawnCount), Mathf.Sin(Mathf.PI * 2 * j /spawnCount));
                
                EnemyProjectile projectile = ObjectPoolManager.Instance.Spawn<EnemyProjectile>(rangeAttackData.projectileData.prefab.name);
        
                projectile.transform.position = (controller.transform.position+(Vector3)rangeAttackData.projectileSpawnOffset) + (Vector3)(direction *rangeAttackData.directionalOffset);
        
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                projectile.transform.eulerAngles = new Vector3(0, 0, angle);
        
                projectile.Init(controller, rangeAttackDataSO, rangeAttackData.projectileData, direction);
                AudioManager.Instance.Play("MonsterProjectileClip");
                
                yield return new WaitForSeconds(Random.Range(rangeAttackData.minSpawnInterval, rangeAttackData.maxSpawnInterval));
            }
            yield return new WaitForSeconds(Random.Range(rangeAttackData.minFireInterval, rangeAttackData.maxFireInterval));
        }
        FireCallback?.Invoke();
    }
}
