using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO_", menuName = "Scriptable Objects/Enemy/EnemyDataSO", order = 0)]
public class EnemyDataSO : CharacterDataSO
{
    public bool isBoss = false;
    public EnemyController enemyPrefab;

    [Space(5f)]
    [Header("Hit Effect")]
    public ParticleHandler hitEffect;
    
    public List<EnemyAttackPattern> attackPatternList;
}

[Serializable]
public class EnemyAttackPattern
{
    public float randomRate;
    public List<EnemyBaseAttackDataSO> attackDatas;
}

