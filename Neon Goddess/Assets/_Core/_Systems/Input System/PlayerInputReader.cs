using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    /// <summary>
    /// Singleton that reads the player Inputs and call events in response
    /// <br></br>Every script subscribed to these events will receive messages when inputs are done
    /// <example> <c>InteractPerformed</c> Action is called when the Interaction button is pressed</example>
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        public static PlayerInputReader Instance;
        
        private InputActions _inputActions;

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Prototype.Interact.performed += OnInteractPerformed;
            _inputActions.Prototype.Movement.performed += OnMovementPerformed;
            _inputActions.Prototype.Movement.canceled += OnMovementCanceled;
            _inputActions.Prototype.Crouch.performed += OnCrouchPerformed;
            _inputActions.Prototype.Aim.performed += OnAimPerformed;
            _inputActions.Prototype.Aim.canceled += OnAimCanceled;
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
            _inputActions.Prototype.Aim.canceled -= OnAimCanceled;
            _inputActions.Prototype.Shoot.performed -= OnShootPerformed;
            
            _inputActions.Disable();
        }

        /// <summary>
        /// Called when Interaction Input is performed
        /// </summary>
        public Action InteractPerformed;
        /// <summary>
        /// Called when Movement Input is performed and passes the Vector2 input
        /// </summary>
        public Action<Vector2> MovementPerformed;
        /// <summary>
        /// Called when Movement Input is canceled (button up)
        /// </summary>
        public Action MovementCanceled;
        /// <summary>
        /// Called when Crouch Input is performed
        /// </summary>
        public Action CrouchPerformed;
        /// <summary>
        /// Called when Aim Input is performed
        /// </summary>
        public Action AimPerformed;
        /// <summary>
        /// Called when Aim Input is canceled
        /// </summary>
        public Action AimCanceled;
        /// <summary>
        /// Called when Shoot Input is performed
        /// </summary>
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
        
        private void OnAimCanceled(InputAction.CallbackContext context)
        {
            AimCanceled?.Invoke();
        }
        
        private void OnShootPerformed(InputAction.CallbackContext context)
        {
            ShootPerformed?.Invoke();
        }
    }
}
