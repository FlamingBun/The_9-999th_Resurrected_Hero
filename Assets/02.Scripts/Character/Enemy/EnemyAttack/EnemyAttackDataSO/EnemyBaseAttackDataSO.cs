using UnityEngine;

public class EnemyBaseAttackDataSO : ScriptableObject
{
    [Header("Common")]
    public string attackName;
    
    public float damageMultiplier;
    
    [Space(5)]
    [Header("Attack Delay")]
    public float attackDelay;
    public float afterAttackDelay;
    
    [Space(5)]
    [Header("Attack Rate")]
    public float attackRate;
    public float attackRateAdjustment;
    
    [Space(5)]
    [Header("Attack Move")]
    public float attackStartRange;
    public float attackStartRangeAdjustment;
    public float attackMoveDistance;
    
    [Space(5)]
    [Header("Knockback")]
    public bool hasKnockback;
    public float knockbackPower;

    [Space(5)]
    [Header("Attack Effect")]
    public GameObject attackEffect;
    
    public GameObject impact;
    public SpriteRenderer indicator;
}
