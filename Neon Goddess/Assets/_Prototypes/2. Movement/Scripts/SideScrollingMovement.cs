using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SideScrollingMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _verticalSpeed;
    [SerializeField] private bool _blockZMovement;
    [SerializeField] private bool _blockAirMovement;
    [SerializeField] private Transform _visualTransform;
    
    private InputActions _inputActions;
    private Vector2 _direction;
    private Vector3 _startPosition;
    private bool _wannaMove;
    
    
    void Awake()
    {
        _inputActions = new InputActions();
        
        _startPosition = transform.position;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Prototype.Movement.performed += PerformMovement;
        _inputActions.Prototype.Movement.canceled += CancelMovement;

        _jumpControl.OnHitTheGround += ResetMovement;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Movement.performed -= PerformMovement;
        _inputActions.Prototype.Movement.canceled -= CancelMovement;
        
        _jumpControl.OnHitTheGround -= ResetMovement;
    }

    private void PerformMovement(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();

        _visualTransform.rotation = Quaternion.Euler(0, 90 * (_direction.x > 0 ? 1 : -1), 0);
        _wannaMove = true;
    }
    
    private void CancelMovement(InputAction.CallbackContext context)
    {
        _direction = Vector2.zero;
        if(!_jumpControl.IsGrounded && _blockAirMovement) return;
        
        _wannaMove = false;

        ResetMovement();
    }

    private void ResetMovement()
    {
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if(!_wannaMove || (!_jumpControl.IsGrounded && _blockAirMovement)) return;
        
        _direction.Normalize();
        var correctDirection = new Vector3(_direction.x * _horizontalSpeed, _rigidbody.velocity.y, _blockZMovement ? 0 : _direction.y * _verticalSpeed);
        _rigidbody.velocity = correctDirection;
    }
    
    public void LockZMovement(bool value)
    {
        _blockZMovement = value;

        if (value)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    public void SetHorizontalSpeed(string speed)
    {
        _horizontalSpeed = float.Parse(speed);
    }

    public void SetVerticalSpeed(string speed)
    {
        _verticalSpeed = float.Parse(speed);
    }

    public void BlockAirMovement(bool value)
    {
        _blockAirMovement = value;
    }
    
    public void ResetPosition()
    {
        transform.position = _startPosition;
        _rigidbody.velocity = Vector3.zero;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
