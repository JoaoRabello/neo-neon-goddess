using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    
    private InputActions _inputActions;

    private bool _wannaMove;
    private bool _canMove = true;
    private Vector3 _movementDirection;
    
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsTurning = Animator.StringToHash("isTurning");
    private static readonly int IsMovingBackwards = Animator.StringToHash("isMovingBackwards");

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Prototype.Movement.performed += MovementPerformed;
        _inputActions.Prototype.Movement.canceled += MovementCanceled;
        
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Movement.performed -= MovementPerformed;
        _inputActions.Prototype.Movement.canceled -= MovementCanceled;
        
        _inputActions.Disable();
    }

    private void MovementPerformed(InputAction.CallbackContext context)
    {
        _movementDirection = context.ReadValue<Vector2>();
        _movementDirection.Normalize();

        _wannaMove = true;
    }
    
    private void MovementCanceled(InputAction.CallbackContext context)
    {
        _movementDirection = Vector3.zero;
        
        _wannaMove = false;
        Stop();
    }

    private void Stop()
    {
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        _animator.SetBool(IsMoving, false);
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
        if (Mathf.Abs(_movementDirection.y) <= 0.1f)
        {
            _animator.SetBool(IsTurning, Mathf.Abs(_movementDirection.x) > 0.1f);
        }
        
        if (!_wannaMove) return;

        var myTransform = transform;
        
        myTransform.Rotate(new Vector3(0, _movementDirection.x * _rotationSpeed, 0));
        
        if (!_canMove) return;

        _animator.SetBool(IsMovingBackwards, _movementDirection.y < 0);

        if (Mathf.Abs(_movementDirection.y) > 0.1f)
            _animator.SetBool(IsMoving, true);
        
        _rigidbody.velocity = myTransform.forward * (_movementDirection.y * _movementSpeed);
    }
}
