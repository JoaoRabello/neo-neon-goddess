using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SideScrollingMovement _horizontalMovementController;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private LayerMask _ledgeLayer;
    [SerializeField] private LayerMask _stepLayer;
    [SerializeField] private Transform _leftLedgeCheckTransform;
    [SerializeField] private Transform _rightLedgeCheckTransform;
    [SerializeField] private Transform _leftClimbTransform;
    [SerializeField] private Transform _rightClimbTransform;
    
    [SerializeField] private Transform _leftStepCheckTransform;
    [SerializeField] private Transform _rightStepCheckTransform;
    [SerializeField] private Transform _leftStepClimbTransform;
    [SerializeField] private Transform _rightStepClimbTransform;
    [SerializeField] private float _checkRadius;

    public bool IsOnLedge;
    
    void Update()
    {
        switch (_jumpControl.IsGrounded)
        {
            case true:
                var stepCheckPosition = _horizontalMovementController.IsMovingRight ? _rightStepCheckTransform.position : _leftStepCheckTransform.position;

                var stepCheck = Physics.OverlapSphere(stepCheckPosition, _checkRadius, _stepLayer);

                if (stepCheck.Length > 0 && Mathf.Abs(_rigidbody.velocity.x) > 0.1f)
                {
                    transform.position =
                        _horizontalMovementController.IsMovingRight ? _rightStepClimbTransform.position : _leftStepClimbTransform.position;
                }
                break;
            case false:
                var ledgeCheckPosition = _horizontalMovementController.IsMovingRight ? _rightLedgeCheckTransform.position : _leftLedgeCheckTransform.position;

                var check = Physics.OverlapSphere(ledgeCheckPosition, _checkRadius, _ledgeLayer);

                if (check.Length > 0)
                {
                    if (!IsOnLedge)
                    {
                        StartCoroutine(ClimbLedge());
                    }
                    IsOnLedge = true;
                }
                else
                {
                    IsOnLedge = false;
                }
                break;
        }
    }

    private IEnumerator ClimbLedge()
    {
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        transform.position =
            _horizontalMovementController.IsMovingRight ? _rightClimbTransform.position : _leftClimbTransform.position;
        
        _rigidbody.useGravity = true;
    }
}
