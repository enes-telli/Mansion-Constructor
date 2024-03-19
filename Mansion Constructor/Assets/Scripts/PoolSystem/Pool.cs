using System.Collections.Generic;
using UnityEngine;
using GameObject = UnityEngine.GameObject;

namespace PoolSystem
{
    public class Pool : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _startAmount;

        public string Name => _name;

        private Queue<GameObject> _objects;

        public void StartPool()
        {
            _objects = new Queue<GameObject>();

            for (int i = 0; i < _startAmount; i++)
            {
                GenerateObjectInPool();
            }
        }

        public void GenerateObjectInPool()
        {
            var obj = Instantiate(_prefab, transform, true);
            obj.SetActive(false);
            _objects.Enqueue(obj);
        }

        public GameObject GetPooledObject()
        {
            if (_objects.Count > 0)
                return _objects.Dequeue();

            return Instantiate(_prefab);
        }

        public void ReturnPooledObject(GameObject obj)
        {
            _objects.Enqueue(obj);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }
    }
}