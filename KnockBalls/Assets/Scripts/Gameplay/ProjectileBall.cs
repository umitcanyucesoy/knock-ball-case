using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBall : MonoBehaviour, IProjectile
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Shoot(Vector3 initialVelocity)
        {
            _rb.velocity = initialVelocity;
            _rb.angularVelocity = Random.insideUnitSphere * 8f; 
        }
    }
}