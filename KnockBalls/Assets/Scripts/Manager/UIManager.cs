using System;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI totalBallCountText;
        
        [Header("Level‑Progress UI")]
        [SerializeField] private Image[] levelSquares;          
        [SerializeField] private TextMeshProUGUI startLevelText; 
        [SerializeField] private TextMeshProUGUI endLevelText;
        
        [Header("State Colors")]
        [SerializeField] private Color[] stateColors = new Color[3]
        {
            Color.green, 
            Color.yellow,
            Color.gray
        };

        private void OnEnable()
        {
            BallShooterController.OnBallCountChanged += UpdateBallCountText;
            LevelManager.OnPhaseIndexChanged += UpdateLevelUI;
        }

        private void OnDisable()
        {
            BallShooterController.OnBallCountChanged -= UpdateBallCountText;
            LevelManager.OnPhaseIndexChanged -= UpdateLevelUI;
        }

        private void UpdateBallCountText(int current, int total)
        {
            totalBallCountText.text = $"{current} / {total}";
        }

        private void UpdateLevelUI(int phaseIndex)
        {
            int currentIdx = Mathf.Clamp(phaseIndex + 1, 0, levelSquares.Length - 1);

            for (int i = 0; i < levelSquares.Length; i++)
            {
                if (i <  currentIdx) levelSquares[i].color = stateColors[0];
                else if (i == currentIdx) levelSquares[i].color = stateColors[1];
                else levelSquares[i].color = stateColors[2]; 
            }

            if (startLevelText)
                startLevelText.text = (phaseIndex + 1).ToString();
            if (endLevelText)
                endLevelText.text   = (phaseIndex + 2).ToString();
        }
    }
}