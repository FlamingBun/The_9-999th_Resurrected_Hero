using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour, IPoolable
{
    private Rigidbody2D _rigid;
    private ObjectPool<Projectile> _pool;
    private Coroutine _fireCoroutine;

    private float _duration;
    private float _damage;

    protected virtual void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
        }
    }

    public void InitState(ObjectPool<Projectile> pool, Vector2 startPos, float damage, float duration)
    {
        transform.position = startPos;
        _pool = pool;
        _damage = damage;
        _duration = duration;
    }

    public void Fire(Vector2 targetDir, float force)
    {
        _rigid.AddForce(targetDir * force, ForceMode2D.Impulse);

        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
        }
        _fireCoroutine = StartCoroutine(FireCoroutine(_duration));
    }

    IEnumerator FireCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        _pool.Return(this);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out StatusHandler targetConditionHandler))
        {
            targetConditionHandler.ModifyStatus(StatType.Health, _damage * - 1);
            
            _pool.Return(this);
        }
    }
}