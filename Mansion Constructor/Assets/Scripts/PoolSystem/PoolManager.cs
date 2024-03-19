using System;
using UnityEngine;

namespace PoolSystem
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;

        [SerializeField] private Pool[] _pools;

        private void Awake()
        {
            Instance = this;

            foreach (var pool in _pools)
            {
                pool.StartPool();
            }
        }

        public T GetPooledObject<T>(string poolName) where T : MonoBehaviour
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                if (_pools[i].Name == poolName)
                {
                    return GetPooledObject<T>(i);
                }
            }

            throw new ArgumentException($"<color=magenta>[MultiPool]</color> There is no pool with the name {poolName}");
        }

        public T GetPooledObject<T>(int index) where T : MonoBehaviour
        {
            var pool = _pools[index];
            return pool.GetPooledObject().GetComponent<T>();
        }

        public void ReturnPooledObject<T>(T obj, string poolName) where T : MonoBehaviour
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                if (_pools[i].Name == poolName)
                {
                    ReturnPooledObject(obj, i);
                    return;
                }
            }

            throw new ArgumentException($"<color=magenta>[MultiPool]</color> There is no pool with the name {poolName}");
        }

        public void ReturnPooledObject<T>(T obj, int index) where T : MonoBehaviour
        {
            var pool = _pools[index];
            pool.ReturnPooledObject(obj.gameObject);
        }
    }
}