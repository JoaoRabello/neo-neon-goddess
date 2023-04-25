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
    private Vector3 _movementDirection;
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

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
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        _animator.SetBool(IsMoving, false);
    }

    private void Update()
    {
        if (!_wannaMove) return;

        _animator.SetBool(IsMoving, true);
        
        var myTransform = transform;
        myTransform.Rotate(new Vector3(0, _movementDirection.x * _rotationSpeed, 0));
        
        _rigidbody.velocity = myTransform.forward * (_movementDirection.y * _movementSpeed);
    }
}
