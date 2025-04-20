using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;

namespace Manager
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private BallShooterController shooterController;
        [SerializeField] private float waitAfterBalls = 2f;

        private int _blocksRemaining;
        private bool _levelEnded;
        private float _ballEndTimer;
        
        private void Awake()
        {
            _blocksRemaining = PhysicalBlock.ActiveCount;
            PhysicalBlock.BlockSpawned += OnBlockSpawned;
            PhysicalBlock.BlockFallen += OnBlockFallen;
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
                NextPhase();
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
                    LosePhase();
            }
            else
                _ballEndTimer = 0f;
        }

        private void NextPhase()
        {
            _levelEnded = true;
            Debug.Log("🎉 LEVEL COMPLETED!");
        }

        private void LosePhase()
        {
            _levelEnded = true;
            Debug.Log("💥 LEVEL FAILED!");
        }
    }
}