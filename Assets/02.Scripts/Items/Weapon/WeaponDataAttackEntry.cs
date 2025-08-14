using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WeaponDataAttackEntry
{
    public float DashDist => dashDist;
    public float DamageMultiplier => damageMultiplier;
    public Vector2 HitBoxSize => hitBoxSize;

    [SerializeField] private float dashDist;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private Vector2 hitBoxSize;
    
}
