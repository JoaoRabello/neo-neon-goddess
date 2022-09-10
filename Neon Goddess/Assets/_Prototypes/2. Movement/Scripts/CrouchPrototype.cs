using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchPrototype : MonoBehaviour
{
    [SerializeField] private SideScrollingMovement _movementController;
    [SerializeField] private JumpControlPrototype _jumpController;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private bool _holdCrouchButton;
    [SerializeField] private Collider _crouchCollider;
    [SerializeField] private Collider _standCollider;
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

        _jumpController.OnJumpPerformed += CancelCrouch;
        _jumpController.OnFallFromGroundWithoutJump += CancelCrouch;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Prototype.Crouch.started -= PerformCrouch;
        _inputActions.Prototype.Crouch.canceled -= PerformReleaseCrouch;
        
        _jumpController.OnJumpPerformed -= CancelCrouch;
        _jumpController.OnFallFromGroundWithoutJump -= CancelCrouch;
    }

    private void PerformCrouch(InputAction.CallbackContext context)
    {
        if (!_jumpController.IsGrounded) return;
        
        if (!_holdCrouchButton)
        {
            SwitchCrouch();
        }
        else
        {
            Crouch();
        }
    }

    private void PerformReleaseCrouch(InputAction.CallbackContext context)
    {
        if (!_holdCrouchButton) return;
        
        CancelCrouch();
    }

    private void Crouch()
    {
        _isCrouching = true;
        
        _movementController.SetSpeed(_crouchSpeed);

        _standCollider.gameObject.SetActive(false);
        _crouchCollider.gameObject.SetActive(true);
        
        _playerAnimator.OnCrouch(true);
    }

    private void CancelCrouch()
    {
        _isCrouching = false;
        
        _movementController.ResetSpeed();
        
        _crouchCollider.gameObject.SetActive(false);
        _standCollider.gameObject.SetActive(true);
        
        _playerAnimator.OnCrouch(false);
    }
    
    private void SwitchCrouch()
    {
        if (!_isCrouching)
        {
            Crouch();
        }
        else
        { 
            CancelCrouch();
        }
    }

    public void SetHoldCrouchButton(bool value)
    {
        _holdCrouchButton = value;
    }
}
