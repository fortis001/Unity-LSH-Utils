using System.Collections.Generic;
using UnityEngine;

public class PrefabVariantPool<T> where T : Component
{
    private readonly IReadOnlyList<T> _prefabs;
    private readonly Transform _poolRoot;

    private readonly Dictionary<T, Queue<T>> _pools = new();
    private readonly Dictionary<T, T> _originPrefabs = new();

    public PrefabVariantPool(IReadOnlyList<T> prefabs, Transform poolRoot)
    {
        _prefabs = prefabs;
        _poolRoot = poolRoot;
    }

    public void Prewarm(int count)
    {
        if (_prefabs == null)
            return;

        foreach (T prefab in _prefabs)
        {
            if (prefab == null)
                continue;

            Queue<T> pool = EnsurePool(prefab);

            for (int i = 0; i < count; i++)
            {
                T instance = CreateNew(prefab);
                instance.gameObject.SetActive(false);
                pool.Enqueue(instance);
            }
        }
    }

    public T Get(T prefab)
    {
        if (prefab == null)
            return null;

        Queue<T> pool = EnsurePool(prefab);

        T instance = pool.Count > 0
            ? pool.Dequeue()
            : CreateNew(prefab);

        instance.transform.SetParent(null);
        instance.gameObject.SetActive(true);

        return instance;
    }

    public void Release(T instance)
    {
        if (instance == null)
            return;

        if (!_originPrefabs.TryGetValue(instance, out T prefab))
        {
            Object.Destroy(instance.gameObject);
            return;
        }

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(_poolRoot);

        EnsurePool(prefab).Enqueue(instance);
    }

    private Queue<T> EnsurePool(T prefab)
    {
        if (!_pools.TryGetValue(prefab, out Queue<T> pool))
        {
            pool = new Queue<T>();
            _pools[prefab] = pool;
        }

        return pool;
    }

    private T CreateNew(T prefab)
    {
        T instance = Object.Instantiate(prefab, _poolRoot);
        _originPrefabs[instance] = prefab;

        return instance;
    }
}