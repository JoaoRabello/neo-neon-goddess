using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform _destinationTransform;
    [SerializeField] private bool _automaticTeleport;
    
    private InputActions _inputActions;

    private bool _isNextToPortal;
    private Transform _playerTransform;

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
        Teleport();
    }

    private void Teleport()
    {
        if (!_isNextToPortal) return;
        Church();
        //_playerTransform.transform.position = _destinationTransform.position;
    }

    public void SetIsNextToPortal(bool value)
    {
        _isNextToPortal = value;
    }

    public void SetPlayerTransform(Collider other)
    {
        _playerTransform = other.transform.parent.transform;

        if (_automaticTeleport && _isNextToPortal) Teleport();
    }

    public void Church()
    {
        SceneManager.LoadScene("church_int", LoadSceneMode.Single);
    }
}
