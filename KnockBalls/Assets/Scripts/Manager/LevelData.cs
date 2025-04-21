using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    [CreateAssetMenu(fileName = "Levels/Level Data", menuName = "LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        [Serializable] public struct LevelObject
        {
            public GameObject prefab;
            public GameObject spawnPointObject;
        }
        
        [Serializable] public struct EntryLevel
        {
            public string levelName;
            public List<LevelObject> levelObjects;
            public int totalBallCount;
        }

        [SerializeField] private List<EntryLevel> levels = new();
        public int Count => levels.Count;
        public List<LevelObject> GetObjects(int i) => levels[i].levelObjects;
        public string GetName(int index) => levels[index].levelName;
        public int GetTotalBallCount(int index) => levels[index].totalBallCount;
    }
}