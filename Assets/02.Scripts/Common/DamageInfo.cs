using UnityEngine;

public struct DamageInfo
{
    public GameObject Attacker { get; private set; }
    public float Damage { get; private set; }
    public Vector3 HitPoint { get; private set; }      
    public Vector3 HitDirection { get; private set; } 
    public float KnockbackForce { get; private set; }
    public bool IsCrit { get; private set; }

    public DamageInfo(GameObject attacker, float damage, Vector3 hitPoint, Vector3 hitDirection, float knockbackForce, bool isCrit = false)
    {
        Damage = damage;
        Attacker = attacker;
        HitPoint = hitPoint;
        HitDirection = hitDirection;
        KnockbackForce = knockbackForce;
        IsCrit = isCrit;
    }
}