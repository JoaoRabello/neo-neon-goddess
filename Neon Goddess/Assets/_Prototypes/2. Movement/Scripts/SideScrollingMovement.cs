using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SideScrollingMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerAnimator _animator;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private LedgeGrabPrototype _ledgeGrabController;
    [SerializeField] private CrouchPrototype _crouchController;
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _fallingHorizontalSpeed;
    [SerializeField] private float _fallingHorizontalSpeedDecreaseRate;
    [SerializeField] private float _verticalSpeed;
    [SerializeField] private float _airMovementCoyoteTime;
    [SerializeField] private bool _blockZMovement;
    [SerializeField] private bool _blockAirMovement;
    [SerializeField] private bool _blockDifferentFallHorizontalSpeed;
    [SerializeField] private Transform _visualTransform;
    
    private InputActions _inputActions;
    private Vector2 _direction;
    private Vector2 _airDirection;
    private Vector3 _startPosition;
    private bool _wannaMove;
    private bool _isMovingRight;
    private bool _canChangeAirDirection = true;
    private bool _isFirstAirDirectionChange = true;
    private bool _useAirMovementDueToJump;

    private float _currentHorizontalSpeed;

    public bool IsMovingRight => _isMovingRight;
    
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
        _jumpControl.OnJumpPerformed += StartAirMovement;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Movement.performed -= PerformMovement;
        _inputActions.Prototype.Movement.canceled -= CancelMovement;
        
        _jumpControl.OnHitTheGround -= ResetMovement;
        _jumpControl.OnJumpPerformed -= StartAirMovement;
    }

    private void PerformMovement(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>().normalized;
        _direction.x = _direction.x switch
        {
            > 0.5f => 1,
            < -0.5f => -1,
            _ => _direction.x
        };
        
        _wannaMove = true;

        _animator.OnMovement(Mathf.Abs(_direction.x) > 0.1f);

        if (_ledgeGrabController.IsOnLedge) return;
        
        SetAirDirection();
        
        if(Mathf.Abs(_direction.x) > 0.1f)
            _isMovingRight = _direction.x > 0;
    }

    private void SetAirDirection()
    {
        if (!_isFirstAirDirectionChange || !_canChangeAirDirection) return;
        
        if(Mathf.Abs(_direction.x) < 0.1f) return;
        
        _visualTransform.rotation = Quaternion.Euler(0, 90 * (_direction.x > 0 ? 1 : -1), 0);
        
        _airDirection = _direction;
        _isFirstAirDirectionChange = false;
    }
    
    private void CancelMovement(InputAction.CallbackContext context)
    {
        _direction = Vector2.zero;
        
        _animator.OnMovement(false);
        
        if(!_jumpControl.IsGrounded && _blockAirMovement) return;
        
        _wannaMove = false;

        ResetVelocity();
    }

    private void StartAirMovement()
    {
        _useAirMovementDueToJump = true;
        _canChangeAirDirection = true;
        _isFirstAirDirectionChange = true;

        _airDirection = _direction;

        StopAllCoroutines();
        StartCoroutine(AirMovementCoyoteTime());
    }

    private IEnumerator AirMovementCoyoteTime()
    {
        yield return new WaitForSeconds(_airMovementCoyoteTime);

        _canChangeAirDirection = false;
    }

    private void ResetVelocity()
    {
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
    }

    private void ResetMovement()
    {
        ResetVelocity();
        _canChangeAirDirection = false;
        _useAirMovementDueToJump = false;
    }

    public void SetSpeed(float newSpeed)
    {
        _currentHorizontalSpeed = newSpeed;
    }

    public void ResetSpeed()
    {
        _currentHorizontalSpeed = _horizontalSpeed;
    }

    private void Update()
    {
        if (!_blockDifferentFallHorizontalSpeed && _rigidbody.velocity.y < -0.1f && !_useAirMovementDueToJump)
        {
            if(_currentHorizontalSpeed > _fallingHorizontalSpeed)
                _currentHorizontalSpeed -= Time.deltaTime * _fallingHorizontalSpeedDecreaseRate;
        }
        else
        {
            if(!_crouchController.IsCrouching)
                _currentHorizontalSpeed = _horizontalSpeed;
        }
        
        Move();
    }

    private void Move()
    {
        if (_ledgeGrabController.IsOnLedge) return;
        
        if(_jumpControl.IsGrounded)
            _visualTransform.rotation = Quaternion.Euler(0, 90 * (_isMovingRight ? 1 : -1), 0);
        
        switch (_jumpControl.IsGrounded)
        {
            case true:
                if (!_wannaMove) return;
                ApplyVelocity(_direction);
                break;
            case false:
                if (_blockAirMovement) return;
                ApplyVelocity(_useAirMovementDueToJump ? _airDirection : _direction);
                break;
        }
    }

    private void ApplyVelocity(Vector3 direction)
    {
        direction.Normalize();
        var correctDirection = new Vector3(direction.x * _currentHorizontalSpeed, _rigidbody.velocity.y, _blockZMovement ? 0 : direction.y * _verticalSpeed);
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
    
    public void SetAirMovementCoyoteTime(string time)
    {
        _airMovementCoyoteTime = float.Parse(time);
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
