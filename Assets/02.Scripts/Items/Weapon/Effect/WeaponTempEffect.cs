using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTempEffect : WeaponBaseEffect
{
    public override void OnAttack(CharacterBaseController owner, CharacterBaseController target)
    {
        var ownerStatHandler = owner.StatHandler;
        
        var poisonDamage = ownerStatHandler.GetStat(StatType.AttackDamage).Value * - 0.1f;
        
        var poisonEffect = new PoisonEffect(poisonDamage, 2, 2);
        
        target.StatusEffectHandler.ApplyEffect(poisonEffect);

        /*Collider2D[] hits = Physics2D.OverlapCircleAll(target.transform.position, 10);

        foreach (var hit in hits)
        {
            if(hit.gameObject == target.gameObject || hit.TryGetComponent(out PlayerController player)) continue;

            if (hit.TryGetComponent(out CharacterStatusHandler targetConditionHandler))
            {
                targetConditionHandler.ModifyCondition(
                    CharacterStatType.Health,
                    weapon.StatHandler.GetStat(WeaponStatType.AttackDamage).Value * -1);
            }
        }*/
    }

    public override void OnDamaged(BaseWeapon source, CharacterBaseController attacker)
    {
    }
}
