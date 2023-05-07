using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LedgeGrab : MonoBehaviour
{
    [SerializeField] private float _grabRange;
    [SerializeField] private float _downRange;
    [SerializeField] private LayerMask _ledgeLayerMask;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private GameObject _parent;
    [SerializeField] private List<Collider> _colliders = new List<Collider>();
    
    private InputActions _inputActions;
    private bool _isClimbing;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Prototype.Ledge.performed += LedgePerformed;
        
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Ledge.performed -= LedgePerformed;
        
        _inputActions.Disable();
    }

    private void OnAnimatorMove()
    {
        if (!_isClimbing) return;
        
        _parent.transform.position += _playerAnimator.deltaPosition;
    }

    private void LedgePerformed(InputAction.CallbackContext obj)
    {
        var hasLedgeInFront = Physics.Raycast(transform.position, transform.forward, out var hit, _grabRange, _ledgeLayerMask);

        if (!hasLedgeInFront) return;
        if (!hit.collider.TryGetComponent<Collider>(out var component)) return;

        StartCoroutine(PlayAndWaitForAnim(_playerAnimator, "Climbing"));
    }
    
    public IEnumerator PlayAndWaitForAnim(Animator targetAnim, string stateName)
    {
        _isClimbing = true;
        targetAnim.Play(stateName);
        
        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }

        while (!targetAnim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        while ((targetAnim.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1 < 0.99f)
        {
            yield return null;
        }

        _isClimbing = false;
        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }
        
        var hasLedgeUnder = Physics.Raycast(transform.position, Vector3.down, out var hit, _downRange, _ledgeLayerMask);

        if (!hasLedgeUnder) yield break;

        transform.position -= transform.position - hit.point;
    }
}
