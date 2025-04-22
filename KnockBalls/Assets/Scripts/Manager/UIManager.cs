using DG.Tweening;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("Level‑Progress UI")]
        [SerializeField] private Image[] levelSquares;
        [SerializeField] private TextMeshProUGUI  startLevelText;
        [SerializeField] private TextMeshProUGUI  endLevelText;
        [SerializeField] private TextMeshProUGUI  totalBallCountText;

        [Header("State Colors")]
        [SerializeField] private Color[] stateColors =
        {
            Color.green, Color.yellow, Color.gray
        };

        [Header("Phase Messages")]
        [SerializeField] private GameObject phaseMessageContainer;
        [SerializeField] private TextMeshProUGUI phaseMessageText;
        [SerializeField] private string[] phaseMessages = { "Awesome!", "Good!", "Perfect!" };
        [SerializeField] public float phaseMsgDuration = 2f;
        [SerializeField] public float tweenDuration = 0.5f;
        [SerializeField] private Ease tweenEase = Ease.OutBack;

        [Header("Level Complete UI")]
        [SerializeField] private GameObject levelCompleteUI;
        [SerializeField] private Button nextButton;

        [Header("Lose UI")]
        [SerializeField] private GameObject losePanel;
        [SerializeField] private Button retryButton;
        
        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            phaseMessageContainer.SetActive(false);
            levelCompleteUI.SetActive(false);
            losePanel.SetActive(false);
        }

        private void OnEnable()
        {
            BallShooterController.OnBallCountChanged += UpdateBallCount;
            LevelManager.OnPhaseIndexChanged += UpdateLevelProgress;
        }

        private void OnDisable()
        {
            BallShooterController.OnBallCountChanged -= UpdateBallCount;
            LevelManager.OnPhaseIndexChanged -= UpdateLevelProgress;
        }
        
        public Button NextButton  => nextButton;
        public Button RetryButton => retryButton;

        public int CycleLen => levelSquares.Length - 1;
        
        public bool IsOverlayActive =>
            phaseMessageContainer.activeSelf || levelCompleteUI.activeSelf || losePanel.activeSelf;

        private void UpdateBallCount(int cur, int total) => 
            totalBallCountText.text = $"{cur} / {total}";

        private void UpdateLevelProgress(int phaseIdx)
        {
            int idx    = phaseIdx % CycleLen;
            int curIdx = idx + 1;

            for (int i = 0; i < levelSquares.Length; i++)
                levelSquares[i].color = i < curIdx ? stateColors[0] :
                                         i == curIdx ? stateColors[1] :
                                                       stateColors[2];

            int cycleNo = phaseIdx / CycleLen + 1;
            startLevelText.text = cycleNo.ToString();
            endLevelText.text   = (cycleNo + 1).ToString();
        }
        
        public bool ShowPhaseMessage(int phaseIdx)
        {
            int idx = phaseIdx % CycleLen;
            if (idx >= phaseMessages.Length)           
                return false;                        

            levelCompleteUI.SetActive(false);
            losePanel.SetActive(false);

            phaseMessageText.text = phaseMessages[idx];
            var rt = phaseMessageContainer.transform;
            phaseMessageContainer.SetActive(true);
            rt.localScale = Vector3.zero;

            rt.DOScale(Vector3.one, tweenDuration)
                .SetEase(tweenEase)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(phaseMsgDuration, () =>
                    {
                        rt.DOScale(Vector3.zero, tweenDuration)
                            .SetEase(tweenEase)
                            .OnComplete(() => phaseMessageContainer.SetActive(false));
                    });
                });

            return true;                               
        }
        
        public void ShowLevelComplete()
        {
            phaseMessageContainer.SetActive(false);
            losePanel.SetActive(false);
            levelCompleteUI.SetActive(true);
            SfxManager.Instance?.Play(SfxManager.SfxType.Win);
        }
        public void HideLevelComplete() => levelCompleteUI.SetActive(false);
        
        public void ShowLosePanel()
        {
            phaseMessageContainer.SetActive(false);
            levelCompleteUI.SetActive(false);
            losePanel.SetActive(true);
            SfxManager.Instance?.Play(SfxManager.SfxType.Lose);
        }
        public void HideLosePanel() => losePanel.SetActive(false);
    }
}