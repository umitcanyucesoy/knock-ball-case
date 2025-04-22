using System;
using UnityEngine;

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
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
            BlockFallen?.Invoke();
        }

        private void OnDestroy()
        {
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
        }
    }
}