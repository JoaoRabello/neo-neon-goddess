using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using Inputs;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerMovements
{
    /// <summary>
    /// Component that controls ledge grab and climbing
    /// </summary>
    public class LedgeGrab : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("Our custom animator")]
        [SerializeField] private CharacterAnimator _animator;
        [Tooltip("Astrid root game object")]
        [SerializeField] private GameObject _parent;
        
        [Header("Raycasts data")]
        [Tooltip("Range to check if ledge wall is close enough")]
        [SerializeField] private LayerMask _ledgeLayerMask;
        [Tooltip("Range to check if ledge wall is close enough")]
        [SerializeField] private float _grabRange;
        [Tooltip("Range to check ground when totally climbed to set player to the ground")]
        [SerializeField] private float _downRange;
        
        [Header("Collision")]
        [Tooltip("All non trigger colliders to turn off when climbing")]
        [SerializeField] private List<Collider> _colliders = new List<Collider>();

        private bool _isClimbing;

        private void OnAnimatorMove()
        {
            if (!_isClimbing) return;

            _parent.transform.position += _animator.DeltaPosition;
        }

        private void OnEnable()
        {
            PlayerInputReader.Instance.InteractPerformed += LedgePerformed;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.InteractPerformed -= LedgePerformed;
        }

        /// <summary>
        /// Checks for walls, look at it and then calls the ledge grab animation
        /// </summary>
        private void LedgePerformed()
        {
            if (_isClimbing) return;
            
            var hasLedgeInFront = Physics.Raycast(transform.position, transform.forward, out var hit, _grabRange,
                _ledgeLayerMask);

            if (!hasLedgeInFront) return;
            if (!hit.collider.TryGetComponent<Collider>(out var collider)) return;

            var lookAtWallDirection = (collider.bounds.center - _parent.transform.position).normalized;
            lookAtWallDirection.y = 0;
            _parent.transform.forward = lookAtWallDirection;
            
            PlayLedgeAnimation();
        }

        /// <summary>
        /// Starts the Ledge animation and setup the callback for when the animation ends
        /// </summary>
        private void PlayLedgeAnimation()
        {
            _isClimbing = true;

            PlayerStateObserver.Instance.OnAnimationStart();

            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }

            _animator.PlayAndOnAnimationEndCallback("Climbing", LedgeAnimationEnded);
        }

        /// <summary>
        /// Ends the ledge climb when the animation has ended
        /// </summary>
        private void LedgeAnimationEnded()
        {
            _isClimbing = false;
            
            PlayerStateObserver.Instance.OnAnimationEnd();
            
            foreach (var collider in _colliders)
            {
                collider.enabled = true;
            }

            var hasLedgeUnder =
                Physics.Raycast(transform.position, Vector3.down, out var hit, _downRange, _ledgeLayerMask);

            if (!hasLedgeUnder) return;

            transform.position -= transform.position - hit.point;
        }
    }
}
