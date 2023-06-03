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
        [SerializeField] private float _grabRange;
        [SerializeField] private float _downRange;
        [SerializeField] private LayerMask _ledgeLayerMask;
        [SerializeField] private CharacterAnimator _animator;
        [SerializeField] private GameObject _parent;
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
