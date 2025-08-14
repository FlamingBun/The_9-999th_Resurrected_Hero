using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDashAttackDataSO", menuName = "Scriptable Objects/Enemy/EnemyAttackData/EnemyDashAttackDataSO", order = 3)]
public class EnemyDashAttackDataSO : EnemyBaseAttackDataSO
{
    [Space(10)]
    [Header("Dash")]
    public EnemyDashAttackData dashAttackData;

    public EnemyWhirlWindAttackData whirlWindData;
}

[Serializable]
public class EnemyDashAttackData
{
    public int maxDashCount;
    
    public float dashRange;
    public float dashDuration;
    public Vector2 dashBoxSize;
    public Vector2 dashBoxOffset;

    public GameObject footSmoke;
}

[Serializable]
public class EnemyWhirlWindAttackData
{
    public float moveSpeed;
    public float obstacleCheckDistance;
    public float yOffset;
    public float whirlWindDuration;
    public float hitInterval;

    public GameObject whirlWindSmoke;
}
