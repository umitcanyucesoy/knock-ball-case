using Cysharp.Threading.Tasks;
using Gameplay;
using UnityEngine;

namespace Manager
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private BallShooterController shooterController;
        [SerializeField] private LevelSpawner levelSpawner;
        [SerializeField] private CannonAnimator cannonAnimator;
        [SerializeField] private float waitAfterBalls = 2f;

        private int   _blocksRemaining;
        private bool  _levelEnded;
        private float _ballEndTimer;
        private int   _phaseIndex;

        public static event System.Action<int> OnPhaseIndexChanged;
        
        private void Awake()
        {
            _blocksRemaining = 0;

            PhysicalBlock.BlockSpawned += OnBlockSpawned;
            PhysicalBlock.BlockFallen  += OnBlockFallen;
        }
        
        private void OnDestroy()
        {
            PhysicalBlock.BlockSpawned -= OnBlockSpawned;
            PhysicalBlock.BlockFallen  -= OnBlockFallen;
        }
        
        private void OnBlockSpawned()  => _blocksRemaining++;
        
        private void OnBlockFallen()
        {
            if (_levelEnded) return;

            _blocksRemaining--;
            if (_blocksRemaining <= 0)
                _ = HandlePhaseCompleteAsync();
        }

        private void Start()
        {
            _phaseIndex = 0;
            levelSpawner.SpawnFirst();
            OnPhaseIndexChanged?.Invoke(_phaseIndex);

            shooterController.SetTotalBallCount(
                levelSpawner.GetCurrentTotalBallCount());
        }

        private void Update()
        {
            if (_levelEnded) return;

            if (shooterController.GetRemainingBallCount() <= 0)
            {
                _ballEndTimer += Time.deltaTime;
                if (_ballEndTimer >= waitAfterBalls)
                    _ = HandleLoseAsync();
            }
            else
                _ballEndTimer = 0f;
        }
        
        private async UniTask HandlePhaseCompleteAsync()
        {
            _levelEnded = true;

            bool messageShown = UIManager.Instance.ShowPhaseMessage(_phaseIndex);

            if (messageShown)
            {
                float wait = UIManager.Instance.tweenDuration * 2f
                             + UIManager.Instance.phaseMsgDuration;
                await UniTask.Delay(System.TimeSpan.FromSeconds(wait));
            }

            int idx = _phaseIndex % UIManager.Instance.CycleLen;

            if (idx < UIManager.Instance.CycleLen - 1)       
            {
                await ContinueToNextPhaseAsync();
            }
            else                                           
            {
                UIManager.Instance.ShowLevelComplete();

                var tcs = new UniTaskCompletionSource();
                void OnNext() => tcs.TrySetResult();
                UIManager.Instance.NextButton.onClick.AddListener(OnNext);
                await tcs.Task;
                UIManager.Instance.NextButton.onClick.RemoveListener(OnNext);
                UIManager.Instance.HideLevelComplete();

                await ContinueToNextPhaseAsync();
            }
        }

        
        private async UniTask HandleLoseAsync()
        {
            _levelEnded = true;
            UIManager.Instance.ShowLosePanel();

            var tcs = new UniTaskCompletionSource();
            void OnRetry() => tcs.TrySetResult();
            UIManager.Instance.RetryButton.onClick.AddListener(OnRetry);
            await tcs.Task;
            UIManager.Instance.RetryButton.onClick.RemoveListener(OnRetry);
            UIManager.Instance.HideLosePanel();

            ObjectPooling.Instance.ReturnAllActiveObjects();

            _blocksRemaining = 0;

            levelSpawner.RespawnCurrent();

            shooterController.SetTotalBallCount(
                levelSpawner.GetCurrentTotalBallCount());
            _ballEndTimer = 0f;
            _levelEnded   = false;

            cannonAnimator.ResetCannon();
        }
        
        private async UniTask ContinueToNextPhaseAsync()
        {
            ObjectPooling.Instance.ReturnAllActiveObjects();

            bool hasNext = levelSpawner.SpawnNext();
            if (hasNext)
            {
                _phaseIndex++;
                OnPhaseIndexChanged?.Invoke(_phaseIndex);
                shooterController.SetTotalBallCount(
                    levelSpawner.GetCurrentTotalBallCount());

                _blocksRemaining = PhysicalBlock.ActiveCount;
                _ballEndTimer    = 0f;
                _levelEnded      = false;
                
                cannonAnimator.ResetCannon();
            }
            else
            {
                Debug.Log("Case Completed!");
            }
        }
        
    }
}
