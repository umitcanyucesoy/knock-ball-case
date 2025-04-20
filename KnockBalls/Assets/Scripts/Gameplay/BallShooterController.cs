using System;
using UnityEngine;

namespace Gameplay
{
    public class BallShooterController : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private ProjectileBall ballPrefab;
        [SerializeField] private Transform ballSpawnPoint;

        [Header("Settings")] 
        [SerializeField] private int totalBallCount;
        [SerializeField] private float projectileSpeed;  
        [SerializeField] private float minHeightCompens;
        
        private Camera _mainCamera;
        private int _currentBallCount;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (!_mainCamera)
                Debug.LogError("No main camera!");

            _currentBallCount = totalBallCount;
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
            ProjectileBall ball = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);

            Vector3  displacement   = target - ballSpawnPoint.position;     
            Vector3  displacementXZ = new Vector3(displacement.x, 0, displacement.z);
            float    distXZ         = displacementXZ.magnitude;                 
            float    yOffset        = displacement.y;                          
            float    g              = Mathf.Abs(Physics.gravity.y);           
            
            if (distXZ < 1f) yOffset += minHeightCompens;

            float speed = projectileSpeed;                                    
            float speed2 = speed * speed;
            float speed4 = speed2 * speed2;
            
            float root = speed4 - g * (g * distXZ * distXZ + 2 * yOffset * speed2);
            if (root < 0f) root = 0f;                                         

            float sqrtRoot = Mathf.Sqrt(root);
            
            float tanTheta = (speed2 - sqrtRoot) / (g * distXZ);
            float angle    = Mathf.Atan(tanTheta);                             
            
            Vector3 velocity =
                displacementXZ.normalized * (speed * Mathf.Cos(angle))        
                + Vector3.up * (speed * Mathf.Sin(angle));                         

            ball.Shoot(velocity);
            _currentBallCount--;
        }

        public int GetRemainingBallCount()
        {
            return _currentBallCount;
        }

        Vector3 GetMouseWorldPoint()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(500f);
        }
    }
}
