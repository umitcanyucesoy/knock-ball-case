using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SfxManager : MonoBehaviour
    {
        public enum SfxType { Shoot, Hit, Win, Lose }
        
        [Serializable] public struct SfxEntry
        {
            public SfxType type;
            public AudioClip clip;
            [Range(0f,1f)] public float volume;
        }
        
        public static SfxManager Instance { get; private set; }

        [Header("Elements")] 
        [SerializeField] private AudioSource oneShotSource;
        [SerializeField] private AudioSource hitSource;
        [SerializeField] private List<SfxEntry> sfxList = new();
        
        private Dictionary<SfxType, SfxEntry> _audioClipDictionary = new();

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            
            if (!oneShotSource) oneShotSource = gameObject.AddComponent<AudioSource>();
            if (!hitSource)     hitSource     = gameObject.AddComponent<AudioSource>();
            
            hitSource.playOnAwake = false;
            oneShotSource.playOnAwake = false;
            
            _audioClipDictionary = new Dictionary<SfxType, SfxEntry>();
            
            foreach (var sfx in sfxList)
                if (sfx.clip) _audioClipDictionary.Add(sfx.type, sfx);
        }
        
        public void Play(SfxType type)
        {
            if (!_audioClipDictionary.TryGetValue(type, out var entry) || !entry.clip)
                return;

            switch (type)
            {
                case SfxType.Hit:
                    if (hitSource.isPlaying) return;
                    hitSource.PlayOneShot(entry.clip, entry.volume);
                    break;

                default:
                    oneShotSource.PlayOneShot(entry.clip, entry.volume);
                    break;
            }
        }
    }
}