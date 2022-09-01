using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchPrototype : MonoBehaviour
{
    [SerializeField] private SideScrollingMovement _movementController;
    [SerializeField] private float _crouchSpeed;
    private InputActions _inputActions;

    private bool _isCrouching;

    public bool IsCrouching => _isCrouching;

    void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Prototype.Crouch.started += PerformCrouch;
        _inputActions.Prototype.Crouch.canceled += PerformReleaseCrouch;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Prototype.Crouch.started -= PerformCrouch;
        _inputActions.Prototype.Crouch.canceled -= PerformReleaseCrouch;
    }

    private void PerformCrouch(InputAction.CallbackContext context)
    {
        Debug.Log("Crouch");
        _isCrouching = true;

        _movementController.SetSpeed(_crouchSpeed);
    }

    private void PerformReleaseCrouch(InputAction.CallbackContext context)
    {
        CancelCrouch();
    }

    private void CancelCrouch()
    {
        _isCrouching = false;
        
        _movementController.ResetSpeed();
    }

    //TODO: Check if crouch is hold or switch (like the example below)
    // private void PerformCrouch(InputAction.CallbackContext context)
    // {
    //     _isCrouching = !_isCrouching;
    //
    //     if (_isCrouching)
    //     {
    //         _movementController.SetSpeed(_crouchSpeed);
    //     }
    //     else
    //     {
    //         _movementController.ResetSpeed();
    //     }
    // }
}
