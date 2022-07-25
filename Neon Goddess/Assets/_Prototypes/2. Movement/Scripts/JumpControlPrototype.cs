using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class JumpControlPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _fallMultiplier;
    [SerializeField] private float _lowFallMultiplier;
    [SerializeField] private bool _fixedPosition;
    
    [Header("Ground Checking")]
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundCheckRadius;

    private InputActions _inputActions;
    private bool _holdingJump;
    private bool _useLongPress = true;
    private bool _isGrounded;
    
    void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Prototype.Jump.started += PerformJump;
        _inputActions.Prototype.Jump.canceled += CancelJump;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Jump.started -= PerformJump;
        _inputActions.Prototype.Jump.canceled -= CancelJump;
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundMask);
        
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if(_rigidbody.velocity.y > 0 && !_holdingJump && _useLongPress)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_lowFallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void PerformJump(InputAction.CallbackContext context)
    {
        if (!_isGrounded) return;
        
        _holdingJump = true;
        
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpHeight, _rigidbody.velocity.z);
    }

    private void CancelJump(InputAction.CallbackContext context)
    {
        _holdingJump = false;
    }

    public void FixJumpPosition(bool value)
    {
        _fixedPosition = value;
    }
    
    public void LockUseLongPress(bool value)
    {
        _useLongPress = value;
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
}
