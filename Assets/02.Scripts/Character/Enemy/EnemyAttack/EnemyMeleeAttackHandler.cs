using System;
using System.Collections;
using UnityEngine;

public class EnemyMeleeAttackHandler
{
    public IEnumerator MeleeAttack(EnemyController controller, Vector2 direction, EnemyMeleeAttackDataSO attackData, Action callback = null)
    {
        GoblinBossController bossController;
        if (controller.IsBoss)
        {
            bossController = controller as GoblinBossController;
        }

        
        EnemyMeleeAttackData meleeWeaponData = attackData.meleeAttackData ;

        bool isMoved = false;
        
        controller.MoveHandler.MakeMove(controller.Rigid, direction, attackData.attackMoveDistance, 0.05f, onEnd: () => isMoved = true);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        
        Vector2 rotatedOffset = rotation * meleeWeaponData.attackOffset;

        bool isHit = false;
        
        while (!isMoved && !isHit)
        {
            Vector2 boxCenter = (Vector2)controller.transform.position + rotatedOffset;
        
            Collider2D[] results = new Collider2D[1];

            if (controller.IsBoss)
            {
                bossController = controller as GoblinBossController;
                //bossController.
            }
            
            if (Physics2D.OverlapBoxNonAlloc(boxCenter, meleeWeaponData.attackRange, angle, results, LayerMask.GetMask("Player")) > 0)
            {
                Collider2D hit = results[0];

                if (hit.TryGetComponent(out PlayerController player))
                {
                    if (player.CanDamageable)
                    {
                        VFXHandler impact = ObjectPoolManager.Instance.Spawn<VFXHandler>(attackData.impact.name);
                
                        impact.transform.position = hit.transform.position;
                
                        impact.Init(attackData.impact.name);
                
                        Vector3 hitDir = hit.transform.position - controller.transform.position;
                        
                        DamageInfo damageInfo = new DamageInfo(controller.gameObject, controller.GetFinalDamage(attackData.damageMultiplier), hit.transform.position, hitDir, 0);
                        
                        player.TakeDamage(damageInfo);

                        isHit = true;
                    
                        if (attackData.hasKnockback)
                        {
                            controller.MoveHandler.MakeMove(player.Rigid, direction, attackData.knockbackPower);
                        }
                    }
                }
            }
            
            yield return null;
        }
        
        callback?.Invoke();
    }
    
}
