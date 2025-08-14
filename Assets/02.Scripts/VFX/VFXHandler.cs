using System;
using UnityEngine;

public class VFXHandler : MonoBehaviour, IPoolable
{
    public static readonly int start = Animator.StringToHash("Start");
    
    protected Animator _animator;

    protected string _poolKey;

    
    protected void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init(string poolKey)
    {
        _poolKey = poolKey;

        _animator.SetTrigger(start);
    }


    public virtual void OnEnd()
    {
        //if (!gameObject.activeInHierarchy) return;
        
        ObjectPoolManager.Instance.Return(_poolKey, this);
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
    }
}
