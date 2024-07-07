using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Player;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionRange;
    [SerializeField] private LayerMask _interactableLayerMask;

    private IInteractable _currentInteractable;

    private List<(IInteractable, Collider)> _knownInteractables = new List<(IInteractable, Collider)>();

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
        var interactableColliders = new Collider[5];

        var interactableObjectCount =
            Physics.OverlapSphereNonAlloc(transform.position, _interactionRange, interactableColliders, _interactableLayerMask);

        if (interactableObjectCount <= 0)
        {
            _currentInteractable = null;

            foreach (var knownInteractable in _knownInteractables.ToList())
            {
                InteractableHUDManager.Instance.RemoveObject(knownInteractable.Item1);
                _knownInteractables.Remove(knownInteractable);
            }
            return;
        }

        for (var index = 0; index < interactableObjectCount; index++)
        {
            var interactableCollider = interactableColliders[index];
            IInteractable interactable;
            
            if (!interactableCollider.TryGetComponent(out interactable))
            {
                if(!interactableCollider.transform.parent.TryGetComponent(out interactable)) continue;
            }
            
            if (index == 0)
            {
                _currentInteractable = interactable;
            }
            
            InteractableHUDManager.Instance.AddObject(interactable, interactableCollider.transform);
            
            if(_knownInteractables.Contains((interactable, interactableCollider))) continue;
            
            _knownInteractables.Add((interactable, interactableCollider));
        }
        
        var closestInteractableDistance = 9999f;
        IInteractable closestInteractable = null;
        
        foreach (var knownInteractable in _knownInteractables)
        {
            var interactableCollider = knownInteractable.Item2;
            var distance = Vector3.Distance(transform.position, interactableCollider.transform.position);
            
            if (distance >= closestInteractableDistance)
            {
                continue;
            }
            
            closestInteractableDistance = distance;
            closestInteractable = knownInteractable.Item1;
        }

        foreach (var knownInteractable in _knownInteractables.Where(knownInteractable => knownInteractable.Item1 != closestInteractable))
        {
            InteractableHUDManager.Instance.RemoveObject(knownInteractable.Item1);
        }

        _currentInteractable = closestInteractable;
    }

    private void TryInteract()
    {
        if(PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnDialogue || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnCamera || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnCutscene) return;
        
        _currentInteractable?.Interact();
    }
}
