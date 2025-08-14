using UnityEngine;

public class EnemyProjectile : EnemyThrowableObject
{
    private void Update()
    {
        Vector3 distanceVector = direction * speed * Time.deltaTime;

        transform.position += distanceVector;

        range -= distanceVector.magnitude;

        if (range < 0f)
        {
            DisableProjectile();
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (layerMask.value == (layerMask.value | (1 << collision.gameObject.layer)))
        {
            if (collision.TryGetComponent(out PlayerController player))
            {
                if (!player.CanDamageable) return;

                if (player.StatusHandler.GetStatus(StatType.Health).CurValue <= 0f) return;

                if (controller == null) return;
                
                Vector3 hitDir = collision.transform.position - controller.transform.position;
                        
                Vector3 hitPoint = collision.ClosestPoint(transform.position);
                
                DamageInfo damageInfo = new DamageInfo(controller.gameObject, damage, hitPoint, hitDir, 0);
                
                player.TakeDamage(damageInfo);
                
                if (hasKnockback)
                {
                    controller.MoveHandler.MakeMove(player.Rigid, direction, knockbackPower);
                }   
            }
            
            DisableProjectile();
        }
    }
    
    private void DisableProjectile()
    {
        // TODO: 사운드 추가
        SpawnImpact();
        
        ObjectPoolManager.Instance.Return(data.prefab.name, this);
    }
}
