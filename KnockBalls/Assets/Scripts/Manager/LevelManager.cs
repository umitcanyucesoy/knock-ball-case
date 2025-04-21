using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;
using Object = System.Object;

namespace Manager
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private BallShooterController shooterController;
        [SerializeField] private LevelSpawner levelSpawner;
        [SerializeField] private float waitAfterBalls = 2f;

        private int _blocksRemaining;
        private bool _levelEnded;
        private float _ballEndTimer;
        private int _phaseIndex;
        
        public static event Action<int> OnPhaseIndexChanged;
        
        private void Awake()
        {
            PhysicalBlock.BlockSpawned += OnBlockSpawned;
            PhysicalBlock.BlockFallen += OnBlockFallen;
            
            _levelEnded = false;
            _blocksRemaining = PhysicalBlock.ActiveCount;
        }

        private void Start()
        {
            _phaseIndex = 0;
            levelSpawner.SpawnFirst();
            OnPhaseIndexChanged?.Invoke(_phaseIndex);
            
            int totalBalls = levelSpawner.GetCurrentTotalBallCount();
            shooterController.SetTotalBallCount(totalBalls);
        }

        private void OnBlockSpawned()
        {
            _blocksRemaining++;
        }

        private void OnBlockFallen()
        {
            if (_levelEnded) return;

            _blocksRemaining--;

            if (_blocksRemaining <= 0)
                HandleWin();
        }

        private void Update()
        {
            ManageLevel();
        }

        private void ManageLevel()
        {
            if (_levelEnded) return;

            if (shooterController.GetRemainingBallCount() <= 0)
            { 
                _ballEndTimer += Time.deltaTime;
                if (_ballEndTimer >= waitAfterBalls)
                    HandleLose();
            }
            else
                _ballEndTimer = 0f;
        }

        private void HandleWin()
        {
            _levelEnded = true;
            
            ObjectPooling.Instance.ReturnAllActiveObjects();
            
            bool hasNext = levelSpawner.SpawnNext();
            if (hasNext)
            {
                _phaseIndex++;
                OnPhaseIndexChanged?.Invoke(_phaseIndex);
                
                PrepareNextPhase();
            }

            Debug.Log("🎉 LEVEL COMPLETED!");
        }

        private void HandleLose()
        {
            _levelEnded = true;
            
            ObjectPooling.Instance.ReturnAllActiveObjects();
            
            Debug.Log("💥 LEVEL FAILED!");
        }

        private void PrepareNextPhase()
        {
            _blocksRemaining = PhysicalBlock.ActiveCount;

            int totalBalls = levelSpawner.GetCurrentTotalBallCount();
            shooterController.SetTotalBallCount(totalBalls);
            
            _ballEndTimer = 0f;
            _levelEnded   = false;
        }
    }
}