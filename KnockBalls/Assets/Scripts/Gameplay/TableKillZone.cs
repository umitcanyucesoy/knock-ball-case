using System;
using Manager;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class TableKillZone : MonoBehaviour
    {
        private void Awake() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IKillZoneAware>(out var block))
            {
                block.MarkAsFallen();
            }
        }
    }
}