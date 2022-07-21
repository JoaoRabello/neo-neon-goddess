using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpControlPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _fallMultiplier;
    [SerializeField] private float _lowFallMultiplier;
    [SerializeField] private bool _fixedPosition;

    private InputActions _inputActions;
    private bool _holdingJump;
    
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
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if(_rigidbody.velocity.y > 0 && !_holdingJump)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_lowFallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void PerformJump(InputAction.CallbackContext context)
    {
        _holdingJump = true;
        
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _fixedPosition ? _jumpHeight : 0, _rigidbody.velocity.z);
    }

    private void CancelJump(InputAction.CallbackContext context)
    {
        _holdingJump = false;
    }
}
