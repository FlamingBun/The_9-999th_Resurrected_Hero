using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMeleeAttackDataSO", menuName = "Scriptable Objects/Enemy/EnemyAttackData/EnemyMeleeAttackDataSO", order = 1)]
public class EnemyMeleeAttackDataSO : EnemyBaseAttackDataSO
{
    [Space(10)]
    [Header("Melee")]
    public EnemyMeleeAttackData meleeAttackData;
}

[Serializable]
public class EnemyMeleeAttackData
{
    public Vector2 attackRange;
    public Vector2 attackOffset;
    public GameObject attackEffect;
}