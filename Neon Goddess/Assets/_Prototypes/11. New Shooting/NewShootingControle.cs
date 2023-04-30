using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewShootingControle : MonoBehaviour
{
    private enum AimDirection
    {
        Up,
        Front,
        Down
    }

    [SerializeField] private TankMovement _tankMovement;
    private AimDirection _currentAimingDirection;
    private Vector3 _currentAimingDirectionVector3;
    private bool _isAiming;
    
    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Prototype.Movement.performed += MovePerformed;
        _inputActions.Prototype.Movement.canceled += MoveCanceled;
        
        _inputActions.Prototype.Aim.performed += AimPerformed;
        _inputActions.Prototype.Aim.canceled += AimCanceled;
        
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Movement.performed -= MovePerformed;
        _inputActions.Prototype.Movement.canceled -= MoveCanceled;
        
        _inputActions.Prototype.Aim.performed -= AimPerformed;
        _inputActions.Prototype.Aim.canceled -= AimCanceled;
        
        _inputActions.Disable();
    }

    private void AimPerformed(InputAction.CallbackContext context)
    {
        //Start Aiming
        _isAiming = true;
        _tankMovement.BlockMovement();
    }
    
    private void MovePerformed(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        
        _currentAimingDirection = input.y switch
        {
            >= 0.1f => AimDirection.Up,
            <= -0.1f => AimDirection.Down,
            _ => AimDirection.Front
        };
    }
    
    private void MoveCanceled(InputAction.CallbackContext obj)
    {
        _currentAimingDirection = AimDirection.Front;
    }

    private void AimCanceled(InputAction.CallbackContext context)
    {
        //Cancel Aiming
        _isAiming = false;
        _tankMovement.UnlockMovement();
    }

    private void Update()
    {
        if (!_isAiming) return;
        
        _currentAimingDirectionVector3 = _currentAimingDirection switch
        {
            AimDirection.Up => transform.forward + Vector3.up * 0.75f,
            AimDirection.Front => transform.forward,
            AimDirection.Down => transform.forward + Vector3.down * 0.75f
        };
    }

    private void OnDrawGizmos()
    {
        if (!_isAiming) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, _currentAimingDirectionVector3 * 3);
    }
}
