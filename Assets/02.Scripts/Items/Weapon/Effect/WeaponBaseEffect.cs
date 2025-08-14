using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseEffect 
{
    public abstract void OnAttack(CharacterBaseController owener, CharacterBaseController target);
    public abstract void OnDamaged(BaseWeapon baseWeapon, CharacterBaseController attacker);
}
