using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using Player;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionRange;
    [SerializeField] private LayerMask _interactableLayerMask;

    private IInteractable _currentInteractable;

    private void OnEnable()
    {
        PlayerInputReader.Instance.InteractPerformed += TryInteract;
    }

    private void OnDisable()
    {
        PlayerInputReader.Instance.InteractPerformed -= TryInteract;
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        var objectList = new Collider[5];

        var interactableObjectCount =
            Physics.OverlapSphereNonAlloc(transform.position, _interactionRange, objectList, _interactableLayerMask);

        if (interactableObjectCount <= 0)
        {
            _currentInteractable = null;
            return;
        }

        _currentInteractable = objectList[0].GetComponent<IInteractable>();
    }

    private void TryInteract()
    {
        if(PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnDialogue) return;
        
        _currentInteractable?.Interact();
    }
}
