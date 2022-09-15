using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimingSystemPrototype : MonoBehaviour
{
    private InputActions _input;
    
    public delegate void AimStartDelegate();

    public delegate void ShootDelegate(GameObject objectShot);

    public event AimStartDelegate OnAimStarted;
    public event ShootDelegate OnShot;

    private void OnEnable()
    {
        _input.Enable();
        
        _input.Prototype.Aim.performed += PerformAim;
        _input.Prototype.Shoot.performed += PerformShoot;
    }
    
    private void OnDisable()
    {
        _input.Disable();
        
        _input.Prototype.Aim.performed -= PerformAim;
        _input.Prototype.Shoot.performed -= PerformShoot;
    }

    private void PerformAim(InputAction.CallbackContext context)
    {
        OnAimStarted?.Invoke();
    }

    private void PerformShoot(InputAction.CallbackContext context)
    {
        OnShot?.Invoke(this.gameObject);
    }
}
