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

    private List<IInteractable> _knownInteractables = new List<IInteractable>();

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

            //TODO: Remover known interactables quando há mais de um também, não só quando todos estão longe
            foreach (var knownInteractable in _knownInteractables.ToList())
            {
                InteractableHUDManager.Instance.RemoveObject(knownInteractable);
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
            
            if(_knownInteractables.Contains(interactable)) continue;
            
            _knownInteractables.Add(interactable);
        }
    }

    private void TryInteract()
    {
        if(PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnDialogue || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnCamera || PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.OnCutscene) return;
        
        _currentInteractable?.Interact();
    }
}
