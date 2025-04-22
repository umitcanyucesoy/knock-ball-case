using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class LevelSpawner : MonoBehaviour
    {
        [SerializeField] private LevelData  levelData;
        [SerializeField] private Transform  anchor;

        private readonly List<GameObject> _spawned = new();
        private int _index;

        public void SpawnFirst() { _index = 0; SpawnCurrent(); }

        public bool SpawnNext()
        {
            _index++;
            if (_index >= levelData.Count) return false;
            SpawnCurrent();
            return true;
        }
        
        public void RespawnCurrent() => SpawnCurrent();
        
        private void SpawnCurrent()
        {
            foreach (var go in _spawned)
                if (go) Destroy(go);
            _spawned.Clear();

            foreach (var obj in levelData.GetObjects(_index))
            {
                if (!obj.prefab || !obj.spawnPointObject) continue;

                Transform sp = obj.spawnPointObject.transform;
                var inst = Instantiate(obj.prefab, sp.position, sp.rotation, anchor);
                _spawned.Add(inst);
            }
        }

        public int GetCurrentTotalBallCount() =>
            levelData.GetTotalBallCount(_index);
    }
}