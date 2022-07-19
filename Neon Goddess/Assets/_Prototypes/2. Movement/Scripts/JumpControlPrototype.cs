using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpControlPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private bool _fixedPosition;

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
        
        _inputActions.Prototype.Jump.started += PerformJump;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Prototype.Jump.started -= PerformJump;
    }

    private void PerformJump(InputAction.CallbackContext context)
    {
        _rigidbody.AddForce(_fixedPosition ? Vector3.up * _jumpHeight : Vector3.zero);
    }
}
