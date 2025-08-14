using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class EnemyThrowableObject : MonoBehaviour, IPoolable
{
    [SerializeField] protected LayerMask layerMask;
    
    protected EnemyProjectileData data;
    protected EnemyController controller;
    
    protected Vector3 direction;
    
    protected string impactName;
    
    protected float damage;
    protected float range;
    protected float speed;
    
    protected bool hasKnockback;
    protected float knockbackPower;
    
    protected abstract void OnTriggerEnter2D(Collider2D collision);
    
    public virtual void Init(EnemyController enemyController, EnemyBaseAttackDataSO attackData, EnemyProjectileData projectileData, Vector2 direction)
    {
        controller = enemyController;
        
        data = projectileData;
        
        impactName = attackData.impact.name;
        
        damage = controller.GetFinalDamage(attackData.damageMultiplier);
        range = Random.Range(data.minRange,data.maxRange);
        speed = Random.Range(data.minSpeed, data.maxSpeed);
        
        this.direction = direction;
        
        hasKnockback = attackData.hasKnockback;
        knockbackPower = attackData.knockbackPower;
    }


    protected void SpawnImpact()
    {
        VFXHandler impact = ObjectPoolManager.Instance.Spawn<VFXHandler>(impactName);
                
        impact.transform.position = transform.position;
                
        impact.Init(impactName);
    }

    public virtual void OnSpawn()
    {
    }

    public virtual void OnDespawn()
    {
    }
}
