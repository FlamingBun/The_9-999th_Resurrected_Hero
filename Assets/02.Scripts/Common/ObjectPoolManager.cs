using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private Dictionary<string, object> _pools = new();


    public ObjectPool<T> CreatePool<T>(T prefab, int defaultCount = 10, Transform parent = null) where T : Component, IPoolable
    {
        string key = prefab.gameObject.name;

        if (_pools.ContainsKey(key)) return _pools[key] as ObjectPool<T>;
        
        if (parent == null)
        {
            parent = transform;
        }
        
        var newPool = new ObjectPool<T>(prefab, defaultCount, parent);

        _pools[key] = newPool;

        return  _pools[key] as ObjectPool<T>;
    }
    

    public T Spawn<T>(string key) where T : Component, IPoolable
    { 
        if (_pools.TryGetValue(key, out var pool))  return ((ObjectPool<T>)pool).Spawn();
      
        return null;
    }
    

    public void Return<T>(string key, T obj) where T : Component, IPoolable
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            ((ObjectPool<T>)pool).Return(obj);
        }
    }

    public void DeSpawnAll(Action callback = null)
    {
        foreach (var pool in _pools.Values)
        {
            IPool ipool = pool as IPool;
            if (ipool != null)
            {
                ipool.ReturnAll();
            }
        }
        
        callback?.Invoke();
    }
}
