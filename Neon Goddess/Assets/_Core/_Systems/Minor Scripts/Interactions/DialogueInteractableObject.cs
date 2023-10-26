using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DialogueInteractableObject : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private IInteractable.InteractableType _interactableType;

    [Header("Dialogue Pool")]
    [SerializeField] private Dialogue[] _dialogues;
    
    [Header("Main Dialogue")]
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private Camera _dialogueCamera;
    
    [Header("Interactions using items")]
    [SerializeField] private bool _itemLockedInteraction;
    [SerializeField] private Item _item;
    [SerializeField] private Dialogue _withoutItemDialogue;
    [SerializeField] private Dialogue _withItemDialogue;
    [SerializeField] private UnityEvent _eventAfterUnlock;

    private bool _hasPlayedEventAfterUnlock;

    [Header("Interaction Count")]
    [SerializeField] private bool _oneTimeInteraction;
    [Header("Order")]    
    [SerializeField] private bool _mainOnly;
    [SerializeField] private bool _randomOnly;
    [SerializeField] private bool _mainThenRandom;

    private int _interactionCount;

    public Action<IInteractable> OnInteractUpdateIcon { get; set; }
    public Action<IInteractable> OnStateChangeUpdateIcon { get; set; }

    public void Interact()
    {
        if(_oneTimeInteraction && _interactionCount >= 1) return;

        if (_itemLockedInteraction)
        {
            if (!PlayerInventoryObserver.Instance.HasItem(_item))
            {
                if (_mainThenRandom)
                {
                    ChatDialogueReader.Instance.PlayDialogue(_interactionCount >= 1 ? GetRandomDialogue() : _withoutItemDialogue, _dialogueCamera);
                }
            }
            else
            {
                if (_hasPlayedEventAfterUnlock) return;
                
                ChatDialogueReader.Instance.PlayDialogue(_withItemDialogue, _dialogueCamera);
                _eventAfterUnlock?.Invoke();

                _hasPlayedEventAfterUnlock = true;
            }
        }
        else
        {
            if (_mainOnly)
            {
                ChatDialogueReader.Instance.PlayDialogue(_dialogue, _dialogueCamera);
            }
            else if (_mainThenRandom)
            {
                ChatDialogueReader.Instance.PlayDialogue(_interactionCount >= 1 ? GetRandomDialogue() : _dialogue, _dialogueCamera);
            }
            else if (_randomOnly)
            {
                ChatDialogueReader.Instance.PlayDialogue(GetRandomDialogue(), _dialogueCamera);
            }
        }

        _interactionCount++;
        OnInteractUpdateIcon?.Invoke(this);
    }

    public IInteractable.InteractableType GetInteractableType()
    {
        return _interactableType;
    }

    public bool HasInteractedOnce()
    {
        return _interactionCount > 0;
    }

    public bool IsLocked()
    {
        return false;
    }

    private Dialogue GetRandomDialogue()
    {
        return _dialogues[Random.Range(0, _dialogues.Length)];
    }
}
