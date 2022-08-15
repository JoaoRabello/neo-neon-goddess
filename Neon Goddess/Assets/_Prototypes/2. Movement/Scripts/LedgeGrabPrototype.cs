using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SideScrollingMovement _horizontalMovementController;
    [SerializeField] private JumpControlPrototype _jumpControl;
    [SerializeField] private LayerMask _ledgeLayer;
    [SerializeField] private Transform _leftLedgeCheckTransform;
    [SerializeField] private Transform _rightLedgeCheckTransform;
    [SerializeField] private Transform _leftClimbTransform;
    [SerializeField] private Transform _rightClimbTransform;
    [SerializeField] private float _checkRadius;

    public bool IsOnLedge;
    
    void Update()
    {
        if (_jumpControl.IsGrounded) return;
        
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
