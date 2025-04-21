using System;
using Gameplay;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI totalBallCountText;

        private void OnEnable()
        {
            BallShooterController.OnBallCountChanged += UpdateBallCountText;
        }

        private void OnDisable()
        {
            BallShooterController.OnBallCountChanged -= UpdateBallCountText;
        }

        private void UpdateBallCountText(int current, int total)
        {
            totalBallCountText.text = $"{current} / {total}";
        }
    }
}