using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class AimingSystemPrototype : MonoBehaviour
{
    [SerializeField] private LineRenderer _aimLine;
    [SerializeField] private AnimationRootMovement _animationRootMovement;
    [SerializeField] private GameObject _hitAim;
    [SerializeField] private Transform _weaponEnd;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _aimInaccuracyMax;
    [SerializeField] private float _aimInaccuracySpeed;
    [SerializeField] private Rig _aimRig;
    [SerializeField] private Rig _torsoRig;
    private InputActions _input;
    
    public delegate void AimStartDelegate();

    public delegate void ShootDelegate(GameObject objectShot);

    public event AimStartDelegate OnAimStarted;
    public event ShootDelegate OnShot;

    private bool _isAiming;
    private GameObject _shotObject;
    private Vector2 _aimInaccuracyOffset;
    private Vector2 _currentInaccuracyOffset;
    private Vector2 _currentInaccuracyVelocity;

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
        TurnAimRiggingOn(true);
    }

    private void CancelAim(InputAction.CallbackContext context)
    {
        _isAiming = false;
        TurnAimLineOn(false);
        TurnAimRiggingOn(false);
    }
    
    private void PerformShoot(InputAction.CallbackContext context)
    {
        if (!_isAiming) return;
        OnShot?.Invoke(_shotObject);
    }

    private void Update()
    {
        var lastInaccuracyOffset = _currentInaccuracyOffset;

        if (Mathf.Abs(_aimInaccuracyOffset.x - _currentInaccuracyOffset.x) < 0.5f || Mathf.Abs(_aimInaccuracyOffset.x - _currentInaccuracyOffset.x) < 0.5f)
        {
            _aimInaccuracyOffset = new Vector2(Random.Range(-_aimInaccuracyMax, _aimInaccuracyMax), Random.Range(-_aimInaccuracyMax, _aimInaccuracyMax));
        }

        _currentInaccuracyOffset = Vector2.SmoothDamp(lastInaccuracyOffset, _aimInaccuracyOffset, ref _currentInaccuracyVelocity, _aimInaccuracySpeed * Time.deltaTime);
        
        var mousePosition = CalculateMousePosition();
        
        _animationRootMovement.SetHandTargetPosition(mousePosition);
        
        AimRaycast(mousePosition);
        DrawAimLine(_hitAim.transform.position);
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

        var targetWithOffset = (Vector2)mousePosition + _currentInaccuracyOffset;
        var ray = new Ray(transform.position, (targetWithOffset - (Vector2)transform.position).normalized);
        
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

    private void TurnAimRiggingOn(bool value)
    {
        _aimRig.weight = value ? 1 : 0;
        _torsoRig.weight = value ? 1 : 0;
    }
    
    private void DrawAimLine(Vector2 lineEndPosition)
    {
        _aimLine.SetPosition(0, _weaponEnd.position);
        _aimLine.SetPosition(1, (Vector2)_weaponEnd.position + (lineEndPosition - (Vector2)_weaponEnd.position).normalized * 2);
    }
}
