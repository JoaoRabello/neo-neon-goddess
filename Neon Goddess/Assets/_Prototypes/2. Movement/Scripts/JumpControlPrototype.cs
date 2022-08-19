using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class JumpControlPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerAnimator _animator;
    [SerializeField] private LedgeGrabPrototype _ledgeGrabController;
    [SerializeField] private float _fallMultiplier;
    [SerializeField] private float _lowFallMultiplier;
    [SerializeField] private float _onJumpGravityMultiplier;
    [SerializeField] private float _coyoteTime;
    [SerializeField] private bool _fixedPosition;
    [SerializeField] private bool _useJumpCooldown;
    [SerializeField] private float _gravity = 10f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _jumpTime = 1f;
    [SerializeField] private float _jumpCooldownTime = 1f;
    
    [Header("Ground Checking")]
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundCheckRadius;

    private InputActions _inputActions;
    private bool _holdingJump;
    private bool _useLongPress = true;
    private bool _isGrounded;
    private bool _hasJumped;
    private bool _jumpOnCooldown;
    private bool _lastFrameWasGrounded;
    private bool _canCoyoteJump = true;
    private bool _coyoteJumpIsLocked = false;

    public bool IsGrounded => _isGrounded;
    private Coroutine _coyoteJumpTimerCoroutine;

    public delegate void HitTheGround();
    public event HitTheGround OnHitTheGround;
    
    void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Prototype.Jump.started += PerformJump;
        _inputActions.Prototype.Jump.canceled += CancelJump;

        OnHitTheGround += StartJumpCooldown;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Jump.started -= PerformJump;
        _inputActions.Prototype.Jump.canceled -= CancelJump;
        
        OnHitTheGround -= StartJumpCooldown;
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundMask);
        
        if(_isGrounded && !_lastFrameWasGrounded) _animator.OnGrounded();

        if (_ledgeGrabController.IsOnLedge) return;
        
        switch (_lastFrameWasGrounded)
        {
            case true when !_isGrounded:
            {
                if(_coyoteJumpTimerCoroutine is not null) StopCoroutine(_coyoteJumpTimerCoroutine);
            
                _coyoteJumpTimerCoroutine = StartCoroutine(CoyoteJumpTimer());
                break;
            }
            case false when _isGrounded:
            {
                OnHitTheGround?.Invoke();
                if(_coyoteJumpTimerCoroutine is not null) StopCoroutine(_coyoteJumpTimerCoroutine);

                if(!_coyoteJumpIsLocked) _canCoyoteJump = true;
                _hasJumped = false;
                break;
            }
        }
        
        _lastFrameWasGrounded = _isGrounded;

        _gravity = -(2 * _jumpHeight) / (Mathf.Pow(_jumpTime, 2));
        
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * _gravity * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if(_rigidbody.velocity.y > 0 && !_holdingJump && _useLongPress)
        {
            _rigidbody.velocity += Vector3.up * _gravity * (_lowFallMultiplier - 1) * Time.deltaTime;
        }
        else if(_rigidbody.velocity.y > 0 && _holdingJump && _useLongPress)
        {
            _rigidbody.velocity += Vector3.up * _gravity * (_onJumpGravityMultiplier - 1) * Time.deltaTime;
        }
    }

    private void PerformJump(InputAction.CallbackContext context)
    {
        if (_jumpOnCooldown || _ledgeGrabController.IsOnLedge) return;
        if ((!_isGrounded && !_canCoyoteJump) || _hasJumped) return;
        
        _holdingJump = true;
        _canCoyoteJump = false;
        _hasJumped = true;
        
        _animator.OnJump();
        
        var verticalVelocity = (2 * _jumpHeight) / _jumpTime;
        
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, verticalVelocity, _rigidbody.velocity.z);
    }

    private void CancelJump(InputAction.CallbackContext context)
    {
        _holdingJump = false;
    }

    private void StartJumpCooldown()
    {
        if (!_useJumpCooldown) return;
        
        StartCoroutine(JumpCooldown());
    }

    private IEnumerator CoyoteJumpTimer()
    {
        if(!_coyoteJumpIsLocked) _canCoyoteJump = true;
        yield return new WaitForSeconds(_coyoteTime);
        _canCoyoteJump = false;
    }

    private IEnumerator JumpCooldown()
    {
        _jumpOnCooldown = true;
        yield return new WaitForSeconds(_jumpCooldownTime);
        _jumpOnCooldown = false;
    }

    public void FixJumpPosition(bool value)
    {
        _fixedPosition = value;
    }
    
    public void LockUseLongPress(bool value)
    {
        _useLongPress = value;
    }
    
    public void UseJumpCooldown(bool value)
    {
        _useJumpCooldown = value;
    }

    public void SetGravityValue(string value)
    {
        _fallMultiplier = float.Parse(value);
    }

    public void SetLongPressGravityValue(string value)
    {
        _lowFallMultiplier = float.Parse(value);
    }

    public void SetJumpForce(string value)
    {
        _jumpHeight = float.Parse(value);
    }

    public void SetUseCoyoteTime(bool value)
    {
        _coyoteJumpIsLocked = !value;
    }

    public void SetCoyoteTime(string value)
    {
        _coyoteTime = float.Parse(value);
    }
    
    public void SetJumpCooldownTimeValue(string value)
    {
        _jumpCooldownTime = float.Parse(value);
    }
}
