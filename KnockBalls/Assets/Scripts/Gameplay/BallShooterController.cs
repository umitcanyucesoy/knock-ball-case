using System;
using Manager;
using UnityEngine;

namespace Gameplay
{
    public class BallShooterController : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private ProjectileBall ballPrefab;
        [SerializeField] private Transform ballSpawnPoint;

        [Header("Settings")] 
        [SerializeField] private float minHeightCompens;
        [SerializeField] private float projectileSpeed;

        [Header("Angle Limits")] 
        [SerializeField] private float inclineAngleDeg = 5f;

        public static event Action<int, int> OnBallCountChanged; 

        private Camera _mainCamera;
        private int _totalBallCount;
        private int _currentBallCount;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (!_mainCamera)
                Debug.LogError("No main camera!");

            _currentBallCount = _totalBallCount;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0) && _currentBallCount > 0)
            {
                Vector3 targetPoint = GetMouseWorldPoint();
                ShootBall(targetPoint);
            }    
        }

        private void ShootBall(Vector3 target)
        {
            ProjectileBall ball = ObjectPooling.Instance.GetPoolObject();
            ball.Prepare(ballSpawnPoint.position, Quaternion.identity);

            Vector3 dir = (target - ballSpawnPoint.position).normalized;

            Vector3 axis = Vector3.Cross(dir, Vector3.up);
            if (axis.sqrMagnitude < 0.001f)
                axis = Vector3.right;

            Quaternion tilt = Quaternion.AngleAxis(inclineAngleDeg, axis);
            Vector3 tiltedDir = tilt * dir;

            Vector3 velocity = tiltedDir * projectileSpeed;
            ball.Shoot(velocity);

            _currentBallCount--;
            OnBallCountChanged?.Invoke(_currentBallCount, _totalBallCount);
        }
        
        public int GetRemainingBallCount()
        {
            return _currentBallCount;
        }
        
        public void SetTotalBallCount(int count)
        {
            _totalBallCount = count;
            _currentBallCount = count;
            
            OnBallCountChanged?.Invoke(_currentBallCount, _totalBallCount);
        }

        Vector3 GetMouseWorldPoint()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(500f);
        }
    }
}
