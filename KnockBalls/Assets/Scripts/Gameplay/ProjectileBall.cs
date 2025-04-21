using System;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBall : MonoBehaviour, IProjectile, IKillZoneAware
    {
        private Rigidbody _rb;
        private bool _shouldBePooled;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Prepare(Vector3 position, Quaternion rotation)
        {
            _rb.position = position;
            _rb.rotation = rotation;

            _rb.velocity        = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.Sleep();
        }

        public void Shoot(Vector3 initialVelocity)
        {
            _rb.WakeUp();
            _rb.velocity        = initialVelocity;
            _rb.angularVelocity = Random.insideUnitSphere * 8f;
        }

        public void ReturnToPool()
        {
            _shouldBePooled = false;
            ObjectPooling.Instance.ReturnToPool(this);
        }

        public void MarkAsFallen()
        {
            _shouldBePooled = true;
        }
        
        public bool ShouldBePooled() => _shouldBePooled;
    }
}