using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _pool = new Queue<T>();
    private T _prefab;
    private Transform _parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (_pool.Count > 0)
        {
            T obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = Object.Instantiate(_prefab, _parent);
            return obj;
        }
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent); // Ensure the bullet is re-parented to the pool manager
        _pool.Enqueue(obj);
    }
}
