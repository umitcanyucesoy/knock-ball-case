using System;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class CannonAnimator : MonoBehaviour
    {
        [Header("Elements")] 
        [SerializeField] private Transform cannonTransform;
        
        [Header("Settings")]
        [SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private float elevationAngle = 15f;
        
        private Quaternion _initialRotation;

        private void Awake() => _initialRotation = cannonTransform.rotation;
        private void OnEnable() => BallShooterController.OnCannonAim += Aim;
        private void OnDisable() => BallShooterController.OnCannonAim -= Aim;

        private void Aim(Vector3 target)
        {
            Vector3 direction = target - cannonTransform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                return;
            
            Quaternion lookRot = Quaternion.LookRotation(direction);
            Quaternion finalRot = lookRot * Quaternion.Euler(-elevationAngle, 0f, 0f);

            cannonTransform
                .DORotateQuaternion(finalRot, rotationDuration)
                .SetEase(Ease.OutSine);
        }
        
        public void ResetCannon()
        {
            cannonTransform.DOKill();
            cannonTransform.rotation = _initialRotation;
        }
    }
}