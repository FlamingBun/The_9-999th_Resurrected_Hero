using System.Collections.Generic;
using UnityEngine;

public interface IPool
{
    public void ReturnAll();
}

public class ObjectPool<T>:IPool where T : Component, IPoolable
{
    private Queue<T> _pool = new ();
    private List<T> _spawnedList = new();
    
    private T _prefab;
    
    private Transform _parent;
    
    public ObjectPool(T prefab, int defaultCount, Transform parent)
    {
        _prefab = prefab;
        
        _parent = parent;
        
        for (int i = 0; i < defaultCount; i++)
        {
            Create();
        }
    }

    private T Create()
    {
        T obj = GameObject.Instantiate(_prefab, _parent);

        obj.gameObject.name = _prefab.gameObject.name;
        obj.gameObject.SetActive(false);
        
        _pool.Enqueue(obj);
        
        return obj;
    }

    public T Spawn()
    {
        T obj = _pool.Count > 0 ? _pool.Dequeue() : Create();
        
        
        obj.gameObject.SetActive(true);
        
        obj.OnSpawn();
        
        _spawnedList.Add(obj);
        
        return obj;
    }

    public void Return(T obj)
    {
        obj.OnDespawn();
        
        obj.transform.SetParent(ObjectPoolManager.Instance.transform);

        obj.gameObject.SetActive(false);
        
        _pool.Enqueue(obj);
        
        _spawnedList.Remove(obj);
    }

    public void ReturnAll()
    {
        for (int i = _spawnedList.Count - 1; i >= 0; i--)
        {
            if (_spawnedList[i].gameObject.activeSelf)
            {
                Return(_spawnedList[i]);
            }
        }
    }
}

