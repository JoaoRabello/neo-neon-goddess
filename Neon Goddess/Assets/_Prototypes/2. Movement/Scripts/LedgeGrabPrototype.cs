using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LedgeGrabPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private AnimationRootMovement _animationRootMovement;
    [SerializeField] private Collider _collider;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private SideScrollingMovement _horizontalMovementController;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private LayerMask _ledgeLayer;
    [SerializeField] private LayerMask _stepLayer;
    [SerializeField] private Transform _leftLedgeCheckTransform;
    [SerializeField] private Transform _rightLedgeCheckTransform;
    [SerializeField] private Transform _leftClimbTransform;
    [SerializeField] private Transform _rightClimbTransform;
    
    [SerializeField] private Transform _leftStepCheckTransform;
    [SerializeField] private Transform _rightStepCheckTransform;
    [SerializeField] private Transform _leftStepClimbTransform;
    [SerializeField] private Transform _rightStepClimbTransform;
    [SerializeField] private float _checkRadius;

    private bool _canClimb;
    private bool _autoClimb;
    private bool _isClimbing;
    public bool IsOnLedge;

    private InputActions _inputActions;
    
    void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Prototype.Jump.started += ProcessClimbInput;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Jump.started -= ProcessClimbInput;
    }
    
    void Update()
    {
        switch (_jumpControl.IsGrounded)
        {
            case true:
                var stepCheckPosition = _horizontalMovementController.IsMovingRight ? _rightStepCheckTransform.position : _leftStepCheckTransform.position;

                var stepCheck = Physics.OverlapSphere(stepCheckPosition, _checkRadius, _stepLayer);

                if (stepCheck.Length > 0 && Mathf.Abs(_rigidbody.velocity.x) > 0.1f)
                {
                    transform.position =
                        _horizontalMovementController.IsMovingRight ? _rightStepClimbTransform.position : _leftStepClimbTransform.position;
                }
                break;
            case false:
                if (_isClimbing || IsOnLedge) return;
                
                var ledgeCheckPosition = _horizontalMovementController.IsMovingRight ? _rightLedgeCheckTransform.position : _leftLedgeCheckTransform.position;

                var check = Physics.OverlapSphere(ledgeCheckPosition, _checkRadius, _ledgeLayer);

                if (check.Length > 0)
                {
                    if (!IsOnLedge)
                    {
                        GrabLedge();
                    }
                    IsOnLedge = true;
                }
                else
                {
                    IsOnLedge = false;
                    _canClimb = false;
                }
                break;
        }
    }

    public void SetAutoClimb(bool value)
    {
        _autoClimb = value;
    }

    private void GrabLedge()
    {
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        
        _canClimb = true;
        
        _playerAnimator.OnLedgeGrab(true);

        if (_autoClimb)
            ClimbLedge();
    }

    private void ProcessClimbInput(InputAction.CallbackContext context)
    {
        if (!_canClimb || _autoClimb) return;
        
        ClimbLedge();
    }

    private void ClimbLedge()
    {
        _canClimb = false;
        
        StopAllCoroutines();
        StartCoroutine(ClimbLedgeRoutine());
    }

    private IEnumerator ClimbLedgeRoutine()
    {
        _isClimbing = true;
        if(_autoClimb)
            yield return new WaitForSeconds(1f);
        
        //TODO: Add _animator and turn on root animation

        _animationRootMovement.SetUseRootAnimation(true);
        _collider.enabled = false;
        _playerAnimator.OnClimbLedge(true);
        
        yield return new WaitForSeconds(0.5f);
        
        _playerAnimator.OnLedgeGrab(false);
        
        yield return new WaitForSeconds(1.3f);
        
        _animationRootMovement.SetUseRootAnimation(false);
        _collider.enabled = true;

        _playerAnimator.OnLedgeGrab(false);
        _playerAnimator.OnClimbLedge(false);
        
        // transform.position =
        //     _horizontalMovementController.IsMovingRight ? _rightClimbTransform.position : _leftClimbTransform.position;
        
        _rigidbody.useGravity = true;
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        
        IsOnLedge = false;
        _canClimb = false;
        _isClimbing = false;
    }
}
