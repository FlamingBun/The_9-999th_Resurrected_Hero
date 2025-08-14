using UnityEngine;
[CreateAssetMenu(fileName = "EnemyCollisionAttackDataSO", menuName = "Scriptable Objects/Enemy/EnemyAttackData/EnemyCollisionAttackDataSO", order = 5)]
public class EnemyCollisionAttackDataSO :EnemyBaseAttackDataSO
{
    [Space(10)]
    [Header("CollisionAttack")]
    public float hitIntervalTime;
}
