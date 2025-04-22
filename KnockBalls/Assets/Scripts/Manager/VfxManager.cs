using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Manager
{
    public class VfxManager : MonoBehaviour
    {
        public enum VfxType { Hit }
        
        [Serializable]
        public class VfxEntry
        {
            public VfxType type;
            public ParticleSystem prefab;
            public float duration = 2f;
        }
        
        [Header("VFX Presets")]
        [SerializeField] private List<VfxEntry> vfxList = new();
        
        private Dictionary<VfxType, VfxEntry> _dict;
        
        public static VfxManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            _dict = new Dictionary<VfxType, VfxEntry>();
            foreach (var e in vfxList)
                if (e.prefab) _dict[e.type] = e;
        }
        
        public void Play(VfxType type, Vector3 position, Quaternion rotation)
        {
            if (!_dict.TryGetValue(type, out var entry) || !entry.prefab) return;

            var ps = Instantiate(entry.prefab, position, rotation);
            Destroy(ps.gameObject, entry.duration);
        }
        
        public void Play(VfxType type, Vector3 position)
            => Play(type, position, Quaternion.identity);
    }
}