using System;
using UnityEngine;

public class ParticleHandler : MonoBehaviour, IPoolable
{
    private string _poolKey;
    

    private ParticleSystem _particleSystem;

    public void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Init(string poolKey, Transform parent = null)
    {
        _poolKey = poolKey;

        if (parent != null)
        {
            transform.SetParent(parent);
        }
        
        _particleSystem.Play();
    }

    public void OnDisable()
    {
        if (_poolKey != null)
        {
            ObjectPoolManager.Instance.Return(_poolKey, this);
        }
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
    }
}