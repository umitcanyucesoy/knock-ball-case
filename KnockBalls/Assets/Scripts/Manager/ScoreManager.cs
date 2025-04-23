using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Manager
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Popup Prefab")]
        [SerializeField] private TextMeshProUGUI popupPrefab;
        [SerializeField] private RectTransform    popupContainer;

        [Header("Animation Settings")]
        [SerializeField] private float floatDistance = 50f;
        [SerializeField] private float floatDuration = 0.5f;
        [SerializeField] private float displayTime   = 0.2f;

        private int _score;
        private int _consecutiveOnes;

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _score = 0;
            _consecutiveOnes = 0;
            if (scoreText) scoreText.text = "0";
        }

        public void AddScore(int amount, Color color, Vector2 screenPosition)
        {
            _score += amount;
            if (scoreText) scoreText.text = _score.ToString();

            ShowFloating(amount, color, screenPosition);

            if (amount == 1)
            {
                _consecutiveOnes++;
                if (_consecutiveOnes >= 7)
                {
                    int bonus = 5;
                    _score += bonus;
                    if (scoreText) scoreText.text = _score.ToString();

                    Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    ShowFloating(bonus, new Color(1f, 0.5f, 0f), center);

                    _consecutiveOnes = 0;
                }
            }
            else
            {
                _consecutiveOnes = 0;
            }
        }

        private void ShowFloating(int amount, Color color, Vector2 screenPosition)
        {
            if (popupPrefab == null || popupContainer == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                popupContainer, screenPosition, null, out Vector2 anchoredPos);

            var go = Instantiate(popupPrefab, popupContainer);
            go.text  = $"+{amount}";
            go.color = color;
            go.raycastTarget = false;
            var rt   = go.rectTransform;
            rt.anchoredPosition = anchoredPos + new Vector2(Random.Range(-20f,20f), 0);
            rt.localScale       = Vector3.one;
            go.alpha            = 1f;

            rt.DOAnchorPosY(anchoredPos.y + floatDistance, floatDuration)
              .SetEase(Ease.OutCubic);
            go.DOFade(0f, floatDuration)
              .SetDelay(displayTime)
              .OnComplete(() => Destroy(go.gameObject));
        }
        
        public void ResetScore()
        {
            _score = 0;
            _consecutiveOnes = 0;
            if (scoreText)
                scoreText.text = "0";
        }
    }
}
