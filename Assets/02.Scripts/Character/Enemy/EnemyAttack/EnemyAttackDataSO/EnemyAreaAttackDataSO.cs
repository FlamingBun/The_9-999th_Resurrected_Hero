using System;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyAreaAttackDataSO", menuName = "Scriptable Objects/Enemy/EnemyAttackData/EnemyAreaAttackDataSO", order = 4)]
public class EnemyAreaAttackDataSO :EnemyBaseAttackDataSO
{
    [Space(10)]
    [Header("JumpAttack")]
    public JumpAttackData jumpAttackData;
    
    [Space(10)]
    [Header("StompAttack")]
    public StompAttackData stompAttackData;
}

[Serializable]
public class JumpAttackData
{
    [Space(5)]
    [Header("Area")]
    public Vector2 areaRange;
    public Vector2 areaOffset;
    
    [Space(5)]
    [Header("InAir")]
    public float chaseTime;
    
    [Space(5)]
    [Header("Jump")]
    public float jumpForce;

    [Space(5)]
    [Header("Dive")]
    public float beforeDiveDelay;
    public float diveDuration;
    
}

[Serializable]
public class StompAttackData
{
    [Space(5)]
    [Header("Stone")]
    public Stone stone;
    
    public Vector2 stoneSize;
    
    public float minOffset;
    public int stoneCount;
    
    public float stoneLifeTime;
}
