using System.Collections.Generic;
using UnityEngine;

namespace LSH.Utils.Pooling
{
    public class SinglePrefabPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _poolRoot;
        private readonly Queue<T> _pool = new();

        public SinglePrefabPool(T prefab, Transform poolRoot)
        {
            _prefab = prefab;
            _poolRoot = poolRoot;
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T instance = CreateNew();
                instance.gameObject.SetActive(false);
                _pool.Enqueue(instance);
            }
        }

        public T Get()
        {
            T instance = _pool.Count > 0
                ? _pool.Dequeue()
                : CreateNew();

            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);

            if (instance is IPoolable poolable)
                poolable.OnGet();

            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null)
                return;

            if (instance is IPoolable poolable)
                poolable.OnRelease();

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_poolRoot);

            _pool.Enqueue(instance);
        }

        private T CreateNew()
        {
            T instance = Object.Instantiate(_prefab, _poolRoot);

            if (instance is IPoolable poolable)
                poolable.OnCreated();

            return instance;
        }
    }
}