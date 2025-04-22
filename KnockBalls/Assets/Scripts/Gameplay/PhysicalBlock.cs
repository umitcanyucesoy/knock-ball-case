using System;
using Manager;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalBlock : MonoBehaviour, IKillZoneAware
    {
        public static int ActiveCount { get; private set; }
        public static event Action BlockSpawned;
        public static event Action BlockFallen;
        
        private bool _hasFallen;

        private void Awake()
        {
            ActiveCount++;
            BlockSpawned?.Invoke();
        }

        public void MarkAsFallen()
        {
            if (_hasFallen) return;
            _hasFallen = true;
            
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
            BlockFallen?.Invoke();
            
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            ScoreManager.Instance?.AddScore(1, Color.white, screenPos);
        }
        
        private void OnDestroy()
        {
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
        }
    }
}