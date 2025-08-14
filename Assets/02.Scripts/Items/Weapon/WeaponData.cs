using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =  "WeaponData", menuName = "Scriptable Objects/Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;

    public WeaponDataAttackEntry[] comboEntries;

    public WeaponDataAttackEntry dodgeAttackEntry;
}
