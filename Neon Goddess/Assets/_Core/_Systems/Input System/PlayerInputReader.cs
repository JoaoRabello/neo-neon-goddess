using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    public class PlayerInputReader : MonoBehaviour
    {
        public static PlayerInputReader Instance;
        
        private InputActions _inputActions;
        
        private void OnEnable()
        {
            _inputActions.Prototype.Interact.performed += OnInteractPerformed;
            _inputActions.Prototype.Movement.performed += OnMovementPerformed;
            _inputActions.Prototype.Movement.canceled += OnMovementCanceled;
            _inputActions.Prototype.Crouch.performed += OnCrouchPerformed;
            _inputActions.Prototype.Aim.performed += OnAimPerformed;
            _inputActions.Prototype.Shoot.performed += OnShootPerformed;
        
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Prototype.Interact.performed -= OnInteractPerformed;
            _inputActions.Prototype.Movement.performed -= OnMovementPerformed;
            _inputActions.Prototype.Movement.canceled -= OnMovementCanceled;
            _inputActions.Prototype.Crouch.performed -= OnCrouchPerformed;
            _inputActions.Prototype.Aim.performed -= OnAimPerformed;
            _inputActions.Prototype.Shoot.performed -= OnShootPerformed;
            
            _inputActions.Disable();
        }

        public Action InteractPerformed;
        public Action<Vector2> MovementPerformed;
        public Action MovementCanceled;
        public Action CrouchPerformed;
        public Action AimPerformed;
        public Action ShootPerformed;
        
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            InteractPerformed?.Invoke();
        }
        
        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            MovementPerformed?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            MovementCanceled?.Invoke();
        }
        
        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            CrouchPerformed?.Invoke();
        }
        
        private void OnAimPerformed(InputAction.CallbackContext context)
        {
            AimPerformed?.Invoke();
        }
        
        private void OnShootPerformed(InputAction.CallbackContext context)
        {
            ShootPerformed?.Invoke();
        }
    }
}
