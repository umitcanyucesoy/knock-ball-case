using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;

namespace Manager
{
    public class ObjectPooling : MonoBehaviour
    {
        [Header("Pooling Settings")]
        [SerializeField] private ProjectileBall poolObjectPrefab;
        [SerializeField] private int poolSize = 50;

        private readonly Queue<ProjectileBall> _pool = new();
        private readonly List<ProjectileBall> _activePoolObjects = new();
        
        public static ObjectPooling Instance { get; private set; }
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            InitPool();
        }

        private void InitPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var poolObj = Instantiate(poolObjectPrefab, transform);
                poolObj.gameObject.SetActive(false);
                _pool.Enqueue(poolObj);
            }
        }

        public void RegisterActiveObject(ProjectileBall activeObject)
        {
            if (!_activePoolObjects.Contains(activeObject)) 
                _activePoolObjects.Add(activeObject);
        }

        public void UnregisterActiveObject(ProjectileBall activeObject)
        {
            _activePoolObjects.Remove(activeObject);
        }

        public ProjectileBall GetPoolObject()
        {
            ProjectileBall poolObj;

            if (_pool.Count > 0)
                poolObj = _pool.Dequeue();
            else
                poolObj = Instantiate(poolObjectPrefab, transform);
            
            poolObj.gameObject.SetActive(true);
            RegisterActiveObject(poolObj);
            return poolObj;
        }

        public void ReturnToPool(ProjectileBall poolObj)
        {
            UnregisterActiveObject(poolObj);
            poolObj.gameObject.SetActive(false);
            poolObj.transform.SetParent(transform);
            _pool.Enqueue(poolObj);
        }
        
        public void ReturnAllActiveObjects()
        {
            var poolObjects = _activePoolObjects.ToList();
            foreach (var obj in poolObjects)
            {
                if (obj.ShouldBePooled())
                 obj.ReturnToPool();
            }
        }
    }
}