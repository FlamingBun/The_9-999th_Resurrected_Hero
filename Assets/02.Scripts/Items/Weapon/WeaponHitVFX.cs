using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WeaponHitVFX : MonoBehaviour, IPoolable
{
    private ObjectPool<WeaponHitVFX> _pool;

    public void Init(ObjectPool<WeaponHitVFX> pool, Vector3 hitPos, Vector3 attackDir, bool isCrit)
    {
        var mat = GetComponent<SpriteRenderer>().material;
        var spriteRenderer = GetComponent<SpriteRenderer>();

        if (isCrit)
        {
            //mat.SetFloat(EnemyMaterialKey.HitEffectBlendKey, 1);
            spriteRenderer.color = Color.red;
            transform.localScale = Vector2.one * 1.1f;
        }
        else
        {
           // mat.SetFloat(EnemyMaterialKey.HitEffectBlendKey, 0);
           spriteRenderer.color = Color.white;
            transform.localScale = Vector2.one;
        }
        
        _pool = pool;
        
        transform.position = hitPos;


        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(0, 0, angle + Random.Range(-30f, 30f));
    }

    public void Finish()
    {
        _pool.Return(this);
    }
    
    public void OnSpawn() { }

    public void OnDespawn() { }
}
