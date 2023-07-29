using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using Combat;
using Inputs;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerMovements
{
    /// <summary>
    /// Component that translates the inputs to tank movements and call animations
    /// <value> <c>• _movementSpeed</c>: represents the frontal and back movement speeds<br></br></value>
    /// <value> <c>• _rotationSpeed</c>: represents the rotational movement speed<br></br></value>
    /// <value> <c>• _wannaMove</c>: represents when a move input performed<br></br></value>
    /// <value> <c>• _canMove</c>: represents when move can be performed<br></br></value>
    /// <value> <c>• _movementDirection</c>: represents the movement input<br></br></value>
    /// </summary>
    public class TankMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AimSystem _aimSystem;
        [Tooltip("Our custom CharacterAnimator")]
        [SerializeField] private CharacterAnimator _animator;
        
        [Header("Movement Data")]
        [Tooltip("Speed for frontal and back movement")]
        [SerializeField] public float _basemovementSpeed;
        [SerializeField] public float _basebackMovementSpeed;
        [Tooltip("Speed for rotational movement")]
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _walkingRotationSpeed;

        private bool _wannaMove;
        private bool _isWalking;
        private bool _canMove = true;
        private Vector3 _movementDirection;

        public float _movementSpeed;
        public float _backMovementSpeed;
        private void OnEnable()
        {
            PlayerInputReader.Instance.MovementPerformed += MovementPerformed;
            PlayerInputReader.Instance.MovementCanceled += MovementCanceled;
            PlayerInputReader.Instance.TurnPerformed += TurnPerformed;
            
            PlayerStateObserver.Instance.AimStart += BlockMovement;
            PlayerStateObserver.Instance.AimEnd += UnlockMovement;
            
            PlayerStateObserver.Instance.AnimationStart += BlockMovement;
            PlayerStateObserver.Instance.AnimationEnd += UnlockMovement;

            _movementSpeed = _basemovementSpeed;
            _backMovementSpeed = _basebackMovementSpeed;

        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.MovementPerformed -= MovementPerformed;
            PlayerInputReader.Instance.MovementCanceled -= MovementCanceled;
            
            PlayerStateObserver.Instance.AimStart -= BlockMovement;
            PlayerStateObserver.Instance.AimEnd -= UnlockMovement;
            
            PlayerStateObserver.Instance.AnimationStart -= BlockMovement;
            PlayerStateObserver.Instance.AnimationEnd -= UnlockMovement;
        }

        /// <summary>
        /// Sets the movement input value and that the player wanna move
        /// </summary>
        /// <param name="movement">Input received as Vector2</param>
        private void MovementPerformed(Vector2 movement)
        {
            _movementDirection = movement;
            _movementDirection.Normalize();

            _wannaMove = true;
        }
        
        /// <summary>
        /// Turns the player on 180 degrees if it's not moving
        /// </summary>
        private void TurnPerformed()
        {
            if (_movementDirection.magnitude > 0.1f) return;

            TurnBody180(transform);
        }

        /// <summary>
        /// Resets the movement input value, sets that the player don't wanna move and stops the player
        /// </summary>
        private void MovementCanceled()
        {
            _movementDirection = Vector3.zero;

            _wannaMove = false;
            Stop();
        }

        /// <summary>
        /// Resets the rigidbody velocities to zero and cancel the moving animation
        /// </summary>
        private void Stop()
        {
            _isWalking = false;
                
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            _animator.SetParameterValue("isMoving", false);
        }

        /// <summary>
        /// Blocks the player movement with _canMove and then stops the player
        /// </summary>
        public void BlockMovement()
        {
            _canMove = false;
            Stop();
        }

        /// <summary>
        /// Free the player movement with _canMove
        /// </summary>
        public void UnlockMovement()
        {
            _canMove = true;
        }

        private void Update()
        {
            MovementProcess();
        }

        /// <summary>
        /// Try to turn with animation.
        /// Then, if wanna move, rotates the body and, at the end,
        /// if can move, moves
        /// </summary>
        private void MovementProcess()
        {
            if (PlayerStateObserver.Instance.CurrentState is not 
                (PlayerStateObserver.PlayerState.Free or PlayerStateObserver.PlayerState.Aiming)) return;

            if(_aimSystem.HasTarget && PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Aiming) return;
            
            TryAnimTurn();

            if (!_wannaMove) return;

            var myTransform = transform;
            RotateBody(myTransform);
            
            if (!_canMove) return;

            Move(myTransform);
        }

        /// <summary>
        /// Moves a rigidbody setting its velocity with the _movementDirection and set animations
        /// </summary>
        /// <param name="myTransform">Player Transform for position tracking</param>
        private void Move(Transform myTransform)
        {
            var isMovingBackwards = _movementDirection.y < 0;
            _animator.SetParameterValue("isMovingBackwards", isMovingBackwards);

            if (Mathf.Abs(_movementDirection.y) > 0.1f)
            {
                _animator.SetParameterValue("isMoving", true);
                _isWalking = true;
            }

            _rigidbody.velocity = myTransform.forward * (_movementDirection.y * (isMovingBackwards ? _backMovementSpeed : _movementSpeed));
        }

        /// <summary>
        /// Rotates the transform around Y axis using the _movementDirection input
        /// </summary>
        /// <param name="myTransform">Player Transform for rotation purposes</param>
        private void RotateBody(Transform myTransform)
        {
            myTransform.Rotate(new Vector3(0, _movementDirection.x * (_isWalking ? _walkingRotationSpeed : _rotationSpeed), 0));
        }
        
        /// <summary>
        /// Rotates the player on 180 degrees
        /// </summary>
        private void TurnBody180(Transform myTransform)
        {
            myTransform.Rotate(new Vector3(0, 180, 0));
        }

        /// <summary>
        /// If only input horizontal, play rotation animations
        /// </summary>
        private void TryAnimTurn()
        {
            if (Mathf.Abs(_movementDirection.y) <= 0.1f)
            {
                _animator.SetParameterValue("isTurning", Mathf.Abs(_movementDirection.x) > 0.1f);
            }
        }
    }

}