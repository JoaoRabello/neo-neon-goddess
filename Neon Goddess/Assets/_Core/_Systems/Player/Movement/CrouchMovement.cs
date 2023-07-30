using System.Collections.Generic;
using Animations;
using Inputs;
using Player;
using UnityEngine;

namespace PlayerMovements
{
    public class CrouchMovement : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _animator;
        
        [Header("Colliders")]
        [SerializeField] private GameObject _standCollider;
        [SerializeField] private List<GameObject> _standCoverColliders = new List<GameObject>();
        [SerializeField] private GameObject _crouchCollider;
        [SerializeField] private List<GameObject> _crouchCoverColliders = new List<GameObject>();

        private bool _isCrouching;
        private TankMovement _Movement;

        public void Start()
        {
            _Movement = GetComponent<TankMovement>();
        }

        private void OnEnable()
        {
            PlayerInputReader.Instance.CrouchPerformed += CrouchPerformed;
            
            PlayerStateObserver.Instance.AimStart += StandUp;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.CrouchPerformed -= CrouchPerformed;
            
            PlayerStateObserver.Instance.AimStart -= StandUp;
        }

        private void CrouchPerformed()
        {
            if (!_isCrouching)
            {
                StartCrouch();
            }
            else
            {
                StandUp();
            }
        }

        private void StartCrouch()
        {
            _isCrouching = true;
            _Movement._movementSpeed = _Movement._basemovementSpeed / 2;
            _Movement._backMovementSpeed = _Movement._basebackMovementSpeed / 2 ;
            _animator.SetParameterValue("isCrouching", true);
            
            SwitchStandColliders(false);
            SwitchCrouchColliders(true);
        }

        private void StandUp()
        {
            _isCrouching = false;
            _Movement._movementSpeed = _Movement._basemovementSpeed;
            _Movement._backMovementSpeed = _Movement._basebackMovementSpeed;
            _animator.SetParameterValue("isCrouching", false);
            
            SwitchCrouchColliders(false);
            SwitchStandColliders(true);
        }

        private void SwitchStandColliders(bool value)
        {
            _standCollider.SetActive(value);
            foreach (var colliderGameObject in _standCoverColliders)
            {
                colliderGameObject.SetActive(value);
            }
        }
        
        private void SwitchCrouchColliders(bool value)
        {
            _crouchCollider.SetActive(value);
            foreach (var colliderGameObject in _crouchCoverColliders)
            {
                colliderGameObject.SetActive(value);
            }
        }
    }
}