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
        
        private void OnCollisionEnter(Collision col)
        {
            if (col.collider.GetComponent<PhysicalBlock>())
            {
                ContactPoint cp = col.GetContact(0);
                VfxManager.Instance?.Play(VfxManager.VfxType.Hit, cp.point, Quaternion.LookRotation(cp.normal));
                SfxManager.Instance?.Play(SfxManager.SfxType.Hit);
            }
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