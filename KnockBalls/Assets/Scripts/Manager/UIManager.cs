using System;
using DG.Tweening;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("Level‑Progress UI")]
        [SerializeField] private Image[] levelSquares;          
        [SerializeField] private TextMeshProUGUI startLevelText; 
        [SerializeField] private TextMeshProUGUI endLevelText;
        [SerializeField] private TextMeshProUGUI totalBallCountText;
        //[SerializeField] private GameObject levelCompleteUI;
        
        [Header("State Colors")]
        [SerializeField] private Color[] stateColors = new Color[3]
        {
            Color.green, 
            Color.yellow,
            Color.gray
        };

        [Header("Phase Messages")]
        [SerializeField] private GameObject phaseMessageContainer;
        [SerializeField] private TextMeshProUGUI phaseMessageText;
        [SerializeField] private string[] phaseMessages = new string[3]
        {
            "Awesome!",
            "Good!",
            "Perfect!"
        };
        
        [Header("Animation Settings")]
        [SerializeField] private float messageTweenDuration = 0.5f;
        [SerializeField] private Ease messageTweenEase = Ease.OutBack;

        private void OnEnable()
        {
            BallShooterController.OnBallCountChanged += UpdateBallCountText;
            LevelManager.OnPhaseIndexChanged += UpdateLevelUI;
            LevelManager.OnPhaseTransitionText += AnimatePhaseMessage;
        }

        private void OnDisable()
        {
            BallShooterController.OnBallCountChanged -= UpdateBallCountText;
            LevelManager.OnPhaseIndexChanged -= UpdateLevelUI;
            LevelManager.OnPhaseTransitionText -= AnimatePhaseMessage;
        }

        private void UpdateBallCountText(int current, int total)
        {
            totalBallCountText.text = $"{current} / {total}";
        }

        private void UpdateLevelUI(int phaseIndex)
        {
            int cycleLen   = levelSquares.Length - 1;       
            int idxInCycle = phaseIndex % cycleLen;           
            int currentIdx = idxInCycle + 1;              
            
            for (int i = 0; i < levelSquares.Length; i++)
            {
                if (i <  currentIdx)     levelSquares[i].color = stateColors[0];
                else if (i == currentIdx) levelSquares[i].color = stateColors[1];
                else                      levelSquares[i].color = stateColors[2]; 
            }

            int cycleNumber = (phaseIndex / cycleLen) + 1; 

            if (startLevelText)
                startLevelText.text = cycleNumber.ToString();
            if (endLevelText)
                endLevelText.text   = (cycleNumber + 1).ToString();
        }

        private void AnimatePhaseMessage(int completedPhaseIndex)
        {
            int cycleLen   = levelSquares.Length - 1;
            int idxInCycle = completedPhaseIndex % cycleLen;

            if (idxInCycle < phaseMessages.Length)
            {
                // if (levelCompleteUI.activeSelf)
                //     levelCompleteUI.SetActive(false);
                phaseMessageContainer.SetActive(true);
                phaseMessageText.text = phaseMessages[idxInCycle];
                var rtIn = phaseMessageContainer.transform;
                rtIn.localScale = Vector3.zero;
                rtIn.DOScale(Vector3.one, messageTweenDuration)
                    .SetEase(messageTweenEase);
                
                DOVirtual.DelayedCall(2.5f, () =>
                {
                    rtIn
                        .DOScale(Vector3.zero, messageTweenDuration)
                        .SetEase(messageTweenEase)
                        .OnComplete(() => phaseMessageContainer.SetActive(false));
                });
            }
            else
            {
                // phaseMessageContainer.SetActive(false);
                // levelCompleteUI.SetActive(true);
                // DOVirtual.DelayedCall(1f, () =>
                // {
                //     levelCompleteUI.SetActive(false);
                // });
            }
        }
    }
}