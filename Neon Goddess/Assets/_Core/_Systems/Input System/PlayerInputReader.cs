using System;
using System.Collections;
using System.Collections.Generic;
using Player;
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
            _inputActions.Astrid.Interaction.started += OnInteractPerformed;
            _inputActions.Astrid.Movement.started += OnMovementStarted;
            _inputActions.Astrid.Movement.performed += OnMovementPerformed;
            _inputActions.Astrid.Movement.canceled += OnMovementCanceled;
            _inputActions.Astrid.Crouch.performed += OnCrouchPerformed;
            _inputActions.Astrid.Aim.performed += OnAimPerformed;
            _inputActions.Astrid.Aim.canceled += OnAimCanceled;
            _inputActions.Astrid.Shoot.performed += OnShootPerformed;
            _inputActions.Astrid.ChangeWeapon.performed += OnChangeWeaponPerformed;
            _inputActions.Astrid.Turn.performed += OnTurnPerformed;
        
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Astrid.Interaction.started -= OnInteractPerformed;
            _inputActions.Astrid.Movement.started -= OnMovementStarted;
            _inputActions.Astrid.Movement.performed -= OnMovementPerformed;
            _inputActions.Astrid.Movement.canceled -= OnMovementCanceled;
            _inputActions.Astrid.Crouch.performed -= OnCrouchPerformed;
            _inputActions.Astrid.Aim.performed -= OnAimPerformed;
            _inputActions.Astrid.Aim.canceled -= OnAimCanceled;
            _inputActions.Astrid.Shoot.performed -= OnShootPerformed;
            _inputActions.Astrid.ChangeWeapon.performed -= OnChangeWeaponPerformed;
            _inputActions.Astrid.Turn.performed -= OnTurnPerformed;
            
            _inputActions.Disable();
        }

        /// <summary>
        /// Called when Interaction Input is performed
        /// </summary>
        public Action InteractPerformed;
        /// <summary>
        /// Called when Movement Input is started and passes the Vector2 input
        /// </summary>
        public Action<Vector2> MovementStarted;
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
        /// <summary>
        /// Called when ChangeWeapon Input is performed
        /// </summary>
        public Action ChangeWeaponPerformed;
        /// <summary>
        /// Called when Turn Input is performed
        /// </summary>
        public Action TurnPerformed;
        
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            InteractPerformed?.Invoke();
        }
        
        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            if (PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Free
                || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Aiming)
            {
                MovementStarted?.Invoke(context.ReadValue<Vector2>());
            }
        }
        
        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            if (PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Free
                || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Aiming)
            {
                MovementPerformed?.Invoke(context.ReadValue<Vector2>());
            }
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
            if (PlayerStateObserver.Instance.CurrentState != PlayerStateObserver.PlayerState.Free) return;
            
            AimPerformed?.Invoke();
        }
        
        private void OnAimCanceled(InputAction.CallbackContext context)
        {
            if (PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Free
                || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Aiming
                || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnAnimation)
            {
                AimCanceled?.Invoke();
            }
        }
        
        private void OnShootPerformed(InputAction.CallbackContext context)
        {
            ShootPerformed?.Invoke();
        }
        
        private void OnChangeWeaponPerformed(InputAction.CallbackContext context)
        {
            ChangeWeaponPerformed?.Invoke();
        }
        
        private void OnTurnPerformed(InputAction.CallbackContext context)
        {
            TurnPerformed?.Invoke();
        }
    }
}
