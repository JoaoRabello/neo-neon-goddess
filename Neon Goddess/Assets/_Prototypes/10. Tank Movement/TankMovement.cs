using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerMovements
{
    public class TankMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [Tooltip("Our custom CharacterAnimator")]
        [SerializeField] private CharacterAnimator _animator;
        
        [Header("Movement Data")]
        [Tooltip("Speed for frontal and back movement")]
        [SerializeField] private float _movementSpeed;
        [Tooltip("Speed for rotational movement")]
        [SerializeField] private float _rotationSpeed;

        private bool _wannaMove;
        private bool _canMove = true;
        private Vector3 _movementDirection;

        private void OnEnable()
        {
            PlayerInputReader.Instance.MovementPerformed += MovementPerformed;
            PlayerInputReader.Instance.MovementCanceled += MovementCanceled;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.MovementPerformed -= MovementPerformed;
            PlayerInputReader.Instance.MovementCanceled -= MovementCanceled;
        }

        private void MovementPerformed(Vector2 movement)
        {
            _movementDirection = movement;
            _movementDirection.Normalize();

            _wannaMove = true;
        }

        private void MovementCanceled()
        {
            _movementDirection = Vector3.zero;

            _wannaMove = false;
            Stop();
        }

        private void Stop()
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            _animator.SetParameterValue("isMoving", false);
        }

        public void BlockMovement()
        {
            _canMove = false;
            Stop();
        }

        public void UnlockMovement()
        {
            _canMove = true;
        }

        private void Update()
        {
            MovementProcess();
        }

        private void MovementProcess()
        {
            TryTurn();

            if (!_wannaMove) return;

            var myTransform = transform;
            RotateBody(myTransform);

            if (!_canMove) return;

            Move(myTransform);
        }

        private void Move(Transform myTransform)
        {
            _animator.SetParameterValue("isMovingBackwards", _movementDirection.y < 0);

            if (Mathf.Abs(_movementDirection.y) > 0.1f)
                _animator.SetParameterValue("isMoving", true);

            _rigidbody.velocity = myTransform.forward * (_movementDirection.y * _movementSpeed);
        }

        private void RotateBody(Transform myTransform)
        {
            myTransform.Rotate(new Vector3(0, _movementDirection.x * _rotationSpeed, 0));
        }

        private void TryTurn()
        {
            if (Mathf.Abs(_movementDirection.y) <= 0.1f)
            {
                _animator.SetParameterValue("isTurning", Mathf.Abs(_movementDirection.x) > 0.1f);
            }
        }
    }

}