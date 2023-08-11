using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private Item _item;
    [SerializeField] private int _amount;
    [SerializeField] private GameObject dialogue;
    private InventoryHolder _playerInventoryHolder;
    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Prototype.Interact.performed += OnInteractPerformed;
        
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Interact.performed -= OnInteractPerformed;
        
        _inputActions.Disable();
    }
    
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (_playerInventoryHolder is null) return;

        if (!_playerInventoryHolder.TryAddItem(_item, _amount)) return;
        
        Destroy(gameObject);
        dialogue.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInventoryHolder = other.GetComponentInParent<InventoryHolder>();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInventoryHolder = null;
        }
    }
}
