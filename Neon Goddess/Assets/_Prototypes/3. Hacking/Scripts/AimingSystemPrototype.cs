using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class AimingSystemPrototype : MonoBehaviour
{
    [Header("Other Systems")]
    [SerializeField] private AnimationRootMovement _animationRootMovement;
    [SerializeField] private SideScrollingMovement _sideScrollingMovement;
    [SerializeField] private LedgeGrabPrototype _ledgeGrabPrototype;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _maxY;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxX;

    [Header("Rendering")]
    [SerializeField] private LineRenderer _aimLine;
    [SerializeField] private RiggingController _riggingController;
    [SerializeField] private GameObject _hitAim;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private Transform _weaponEnd;
    [SerializeField] private Transform _visualTransform;

    [Header("Precision System")]
    [SerializeField] private float _stablePrecisionError;
    [SerializeField] private float _affectedPercentPrecisionError;
    [SerializeField] private float _stablePrecisionErrorSpeed;
    [SerializeField] private float _affectedPercentPrecisionErrorSpeed;
    
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

        _animationRootMovement.OnWieldAnimationComplete += OnWieldComplete;
        _animationRootMovement.OnGetWeaponFromPocket += DrawWeapon;
        
        _animationRootMovement.OnHideAnimationComplete += OnHideWeaponComplete;
        _animationRootMovement.OnHideWeaponOnPocket += HideWeapon;
    }
    
    private void OnDisable()
    {
        _input.Disable();
        
        _input.Prototype.Aim.performed -= PerformAim;
        _input.Prototype.Aim.canceled -= CancelAim;
        _input.Prototype.Shoot.performed -= PerformShoot;
        
        _jumpControl.OnJumpPerformed -= CancelAimDueToMovements;
        
        _sideScrollingMovement.OnStartMoving -= CancelAimDueToMovements;
        
        _animationRootMovement.OnWieldAnimationComplete -= OnWieldComplete;
        _animationRootMovement.OnGetWeaponFromPocket -= DrawWeapon;
        
        _animationRootMovement.OnHideAnimationComplete -= OnHideWeaponComplete;
        _animationRootMovement.OnHideWeaponOnPocket -= HideWeapon;
    }

    private void PerformAim(InputAction.CallbackContext context)
    {
        if (_sideScrollingMovement.IsMoving || _ledgeGrabPrototype.IsOnLedge || !_jumpControl.IsGrounded) return;
        
        OnAimStarted?.Invoke();

        _isAiming = true;
        
        WieldWeapon();
    }

    private void CancelAimDueToMovements()
    {
        CancelAim(new InputAction.CallbackContext());
    }

    private void CancelAim(InputAction.CallbackContext context)
    {
        _isAiming = false;
        TurnAimLineOn(false);
        _riggingController.TurnAimRiggingOn(false);

        StartHideWeapon();
    }

    private void OnWieldComplete()
    {
        _riggingController.TurnOnHandsRigging();
        _riggingController.TurnAimRiggingOn(true);
        
        TurnAimLineOn(true);
    }

    private void DrawWeapon()
    {
        _weapon.SetActive(true);
    }

    private void HideWeapon()
    {
        _weapon.SetActive(false);
    }

    private void WieldWeapon()
    {
        _playerAnimator.OnWeaponWield(true);
    }

    private void OnHideWeaponComplete()
    {
        _playerAnimator.OnWeaponWield(false);
    }

    private void StartHideWeapon()
    {
        _riggingController.TurnOffRigs();
        
        _playerAnimator.OnWeaponHide();
    }
    
    private void PerformShoot(InputAction.CallbackContext context)
    {
        if (!_isAiming) return;
        OnShot?.Invoke(_shotObject);
        
        _playerAnimator.OnShoot();
    }

    private void Update()
    {
        if (!_isAiming) return;
        
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
        var precisionErrorSpeed = _stablePrecisionErrorSpeed * (1 - _affectedPercentPrecisionErrorSpeed / 100);

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

        mousePositionCorrected.y = Mathf.Clamp(mousePositionCorrected.y, transform.position.y + _minY, transform.position.y + _maxY);
        if (mousePositionCorrected.x < transform.position.x)
        {
            mousePositionCorrected.x = Mathf.Clamp(mousePositionCorrected.x, mousePositionCorrected.x, transform.position.x - _maxX);
        }
        else
        {
            mousePositionCorrected.x = Mathf.Clamp(mousePositionCorrected.x, transform.position.x + _maxX, mousePositionCorrected.x);
        }

        return mousePositionCorrected;
    }

    private void AimRaycast(Vector3 mousePosition)
    {
        var rayHit = new RaycastHit();

        var targetWithOffset = (Vector2)mousePosition + _currentPrecisionError;

        var direction = (targetWithOffset - (Vector2) transform.position).normalized;
        
        var ray = new Ray(transform.position, direction);
        
        if(Physics.Raycast(ray, out rayHit, 20))
        {
            _hitAim.transform.position = rayHit.point;
            _shotObject = rayHit.collider.gameObject;
        }
        else
        {
            _hitAim.transform.position = (Vector2)transform.position + direction * 20;
        }
        
        StartCoroutine(Rotate(targetWithOffset.x));
        Debug.Log(_visualTransform.rotation);
    }

    private IEnumerator Rotate(float value)
    {
        yield return null;
        _visualTransform.rotation = Quaternion.Euler(0, 90 * (value > transform.position.x ? 1 : -1), 0);
    }

    private void TurnAimLineOn(bool value)
    {
        _aimLine.gameObject.SetActive(value);
        _hitAim.SetActive(value);
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
        _stablePrecisionErrorSpeed = float.Parse(value);
    }
    
    public void SetAffectedPercentPrecisionErrorSpeed(string value)
    {
        _affectedPercentPrecisionErrorSpeed = float.Parse(value);
    }
}