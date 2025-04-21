using System;
using UnityEngine;
using Manager;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalBlock : MonoBehaviour, IKillZoneAware
    {
        public static int ActiveCount { get; private set; }
        public static event Action BlockSpawned;
        public static event Action BlockFallen;

        private void Awake()
        {
            ActiveCount++;
            BlockSpawned?.Invoke();
        }

        public void MarkAsFallen()
        {
            ActiveCount--;
            BlockFallen?.Invoke();
        }
    }
}
