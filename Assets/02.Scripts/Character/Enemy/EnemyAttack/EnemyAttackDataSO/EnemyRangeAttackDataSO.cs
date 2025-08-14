using System;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyRangeAttackDataSO", menuName = "Scriptable Objects/Enemy/EnemyAttackData/EnemyRangeAttackDataSO", order = 2)]
public class EnemyRangeAttackDataSO : EnemyBaseAttackDataSO
{
    [Space(10)]
    [Header("Range")]
    public EnemyRangeAttackData rangeAttackData;
}
[Serializable]
public class EnemyRangeAttackData
{
    [Header("Common")]
    [Space(10)]
    [Header("Fire And Spawn")]
    public int fireCount;
    public int spawnCount;

    public float minFireInterval;
    public float maxFireInterval;

    public float minSpawnInterval;
    public float maxSpawnInterval;
    
    [Space(6)]
    [Header("Offset")]
    public float directionalOffset;
    public Vector2 projectileSpawnOffset;
    
    [Space(10)]
    [Header("Normal Fire")]
    public float minSpread;
    public float maxSpread;
    
    [Header("Cross Fire")]
    public float randomAngleRange;

    [Header("Bomb Fire")]
    public float bombFlightTime;
    public float bombArcPeakHeight;
    public GameObject smokeEffect;
    
    [Space(10)]
    [Header("Goblin Boss Fire")]
    public float rotationSpeed;

    public bool directionFlipable;
    
    [Space(10)]
    [Header("Projectile")]
    public EnemyProjectileData projectileData;
}

[Serializable]
public class EnemyProjectileData
{
    public GameObject prefab;
    public float minSpeed;
    public float maxSpeed;
    public float minRange;
    public float maxRange; // 최대 이동거리
}