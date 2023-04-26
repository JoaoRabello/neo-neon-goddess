using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LedgeGrab : MonoBehaviour
{
    [SerializeField] private float _grabRange;
    [SerializeField] private LayerMask _ledgeLayerMask;
    private InputActions _inputActions;

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
    
    private void LedgePerformed(InputAction.CallbackContext obj)
    {
        var hasLedgeInFront = Physics.Raycast(transform.position, transform.forward, out var hit, _grabRange, _ledgeLayerMask);

        if (!hasLedgeInFront) return;
        if (!hit.collider.TryGetComponent<Collider>(out var component)) return;
        
        Debug.Log(component.name);
    }
}
