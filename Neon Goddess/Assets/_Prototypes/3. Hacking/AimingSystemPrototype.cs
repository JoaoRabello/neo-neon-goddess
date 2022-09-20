using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimingSystemPrototype : MonoBehaviour
{
    [SerializeField] private LineRenderer _aimLine;
    [SerializeField] private AnimationRootMovement _animationRootMovement;
    [SerializeField] private GameObject _hitAim;
    [SerializeField] private Camera _mainCamera;
    private InputActions _input;
    
    public delegate void AimStartDelegate();

    public delegate void ShootDelegate(GameObject objectShot);

    public event AimStartDelegate OnAimStarted;
    public event ShootDelegate OnShot;

    private bool _isAiming;
    private GameObject _shotObject;

    private void Awake()
    {
        _input = new InputActions();
    }
    
    private void OnEnable()
    {
        _input.Enable();
        
        _input.Prototype.Aim.performed += PerformAim;
        _input.Prototype.Aim.canceled += CancelAim;
        _input.Prototype.Shoot.performed += PerformShoot;
    }
    
    private void OnDisable()
    {
        _input.Disable();
        
        _input.Prototype.Aim.performed -= PerformAim;
        _input.Prototype.Aim.canceled -= CancelAim;
        _input.Prototype.Shoot.performed -= PerformShoot;
    }

    private void PerformAim(InputAction.CallbackContext context)
    {
        OnAimStarted?.Invoke();

        _isAiming = true;
        TurnAimLineOn(true);
    }

    private void CancelAim(InputAction.CallbackContext context)
    {
        _isAiming = false;
        TurnAimLineOn(false);
    }
    
    private void PerformShoot(InputAction.CallbackContext context)
    {
        if (!_isAiming) return;
        OnShot?.Invoke(_shotObject);
    }

    private void Update()
    {
        var mousePosition = CalculateMousePosition();
        
        _animationRootMovement.SetHandTargetPosition(mousePosition);
        
        AimRaycast(mousePosition);
        DrawAimLine(mousePosition);
    }

    private Vector3 CalculateMousePosition()
    {
        var mousePosition = (Vector3)Mouse.current.position.ReadValue();
        mousePosition.z = -_mainCamera.transform.position.z;
        var mousePositionCorrected = _mainCamera.ScreenToWorldPoint(mousePosition);
        mousePositionCorrected.z = 0;

        return mousePositionCorrected;
    }

    private void AimRaycast(Vector3 mousePosition)
    {
        var rayHit = new RaycastHit();
        var ray = new Ray(transform.position, (mousePosition - transform.position).normalized);
        
        if(Physics.Raycast(ray, out rayHit, 20))
        {
            _hitAim.transform.position = rayHit.point;
            _shotObject = rayHit.collider.gameObject;
        }
    }

    private void TurnAimLineOn(bool value)
    {
        _aimLine.gameObject.SetActive(value);
        _hitAim.SetActive(value);
    }
    
    private void DrawAimLine(Vector2 mousePosition)
    {
        _aimLine.SetPosition(0, transform.position);
        _aimLine.SetPosition(1, (Vector2)transform.position + (mousePosition - (Vector2)transform.position).normalized * 2);
    }
}
