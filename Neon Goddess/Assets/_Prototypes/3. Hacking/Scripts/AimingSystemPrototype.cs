using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class AimingSystemPrototype : MonoBehaviour
{
    [SerializeField] private LineRenderer _aimLine;
    [SerializeField] private AnimationRootMovement _animationRootMovement;
    [SerializeField] private SideScrollingMovement _sideScrollingMovement;
    [SerializeField] private LedgeGrabPrototype _ledgeGrabPrototype;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private GameObject _hitAim;
    [SerializeField] private Transform _weaponEnd;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _stablePrecisionError;
    [SerializeField] private float _affectedPercentPrecisionError;
    [SerializeField] private float _precisionErrorSpeed;
    [SerializeField] private Rig _aimRig;
    [SerializeField] private Rig _torsoRig;
    private InputActions _input;
    
    public delegate void AimStartDelegate();

    public delegate void ShootDelegate(GameObject objectShot);

    public event AimStartDelegate OnAimStarted;
    public event ShootDelegate OnShot;

    private bool _isAiming;
    private GameObject _shotObject;
    private Vector2 _aimPrecisionError;
    private Vector2 _currentPrecisionError;
    private Vector2 _currentPrecisionErrorVelocity;

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

        _jumpControl.OnJumpPerformed += CancelAimDueToMovements;

        _sideScrollingMovement.OnStartMoving += CancelAimDueToMovements;
    }
    
    private void OnDisable()
    {
        _input.Disable();
        
        _input.Prototype.Aim.performed -= PerformAim;
        _input.Prototype.Aim.canceled -= CancelAim;
        _input.Prototype.Shoot.performed -= PerformShoot;
        
        _jumpControl.OnJumpPerformed -= CancelAimDueToMovements;
        
        _sideScrollingMovement.OnStartMoving -= CancelAimDueToMovements;
    }

    private void PerformAim(InputAction.CallbackContext context)
    {
        if (_sideScrollingMovement.IsMoving || _ledgeGrabPrototype.IsOnLedge || !_jumpControl.IsGrounded) return;
        
        OnAimStarted?.Invoke();

        _isAiming = true;
        TurnAimLineOn(true);
        TurnAimRiggingOn(true);
    }

    private void CancelAimDueToMovements()
    {
        CancelAim(new InputAction.CallbackContext());
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
        CalculateAimPrecision();

        var mousePosition = CalculateMousePosition();
        
        _animationRootMovement.SetHandTargetPosition(mousePosition);
        
        AimRaycast(mousePosition);
        DrawAimLine(_hitAim.transform.position);
    }

    private void CalculateAimPrecision()
    {
        var lastPrecisionError = _currentPrecisionError;
        var precisionError = _stablePrecisionError * (1 + _affectedPercentPrecisionError/100);
        var precisionErrorSpeed = _precisionErrorSpeed * (1 - _affectedPercentPrecisionError / 100);

        if (Mathf.Abs(_aimPrecisionError.x - _currentPrecisionError.x) < 0.2f || Mathf.Abs(_aimPrecisionError.y - _currentPrecisionError.y) < 0.2f)
        {
            CalculateRandomAimPrecisionError(precisionError);
        }

        _currentPrecisionError = Vector2.SmoothDamp(lastPrecisionError, _aimPrecisionError, ref _currentPrecisionErrorVelocity, precisionErrorSpeed * Time.deltaTime);
    }

    private void CalculateRandomAimPrecisionError(float precisionError)
    {
        _aimPrecisionError = new Vector2(Random.Range(-precisionError, precisionError), Random.Range(-precisionError, precisionError));
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

        var targetWithOffset = (Vector2)mousePosition + _currentPrecisionError;
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

    public void SetStablePrecisionError(string value)
    {
        _stablePrecisionError = float.Parse(value);
    }
    
    public void SetAffectedPercentPrecisionError(string value)
    {
        _affectedPercentPrecisionError = float.Parse(value);
    }

    public void SetPrecisionErrorSpeed(string value)
    {
        _precisionErrorSpeed = float.Parse(value);
    }
}
