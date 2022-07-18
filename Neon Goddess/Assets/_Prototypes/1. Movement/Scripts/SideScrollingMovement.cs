using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SideScrollingMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed;
    
    private InputActions _inputActions;
    private Vector2 _direction;
    private bool _wannaMove;
    
    void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Prototype.Movement.performed += PerformMovement;
        _inputActions.Prototype.Movement.canceled += CancelMovement;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Movement.performed -= PerformMovement;
        _inputActions.Prototype.Movement.canceled -= CancelMovement;
    }

    private void PerformMovement(InputAction.CallbackContext context)
    {
        _wannaMove = true;
        
        _direction = context.ReadValue<Vector2>();
    }
    
    private void CancelMovement(InputAction.CallbackContext context)
    {
        _wannaMove = false;

        _rigidbody.velocity = Vector2.zero;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if(!_wannaMove) return;
        
        var correctDirection = new Vector3(_direction.x, 0, 0);
        _rigidbody.velocity = correctDirection.normalized * _speed;
    }
}
