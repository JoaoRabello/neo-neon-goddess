using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DialogueInteractableObject : MonoBehaviour, IInteractable
{
    [Header("Dialogue Pool")]
    [SerializeField] private Dialogue[] _dialogues;
    
    [Header("Main Dialogue")]
    [SerializeField] private Dialogue _dialogue;

    [Header("Interaction Count")]
    [SerializeField] private bool _oneTimeInteraction;
    [Header("Order")]    
    [SerializeField] private bool _mainOnly;
    [SerializeField] private bool _randomOnly;
    [SerializeField] private bool _mainThenRandom;

    private int _interactionCount;
    
    public void Interact()
    {
        if(_oneTimeInteraction && _interactionCount >= 1) return;

        if (_mainOnly)
        {
            ChatDialogueReader.Instance.PlayDialogue(_dialogue);
        }
        else if (_mainThenRandom)
        {
            ChatDialogueReader.Instance.PlayDialogue(_interactionCount >= 1 ? GetRandomDialogue() : _dialogue);
        }
        else if (_randomOnly)
        {
            ChatDialogueReader.Instance.PlayDialogue(GetRandomDialogue());
        }

        _interactionCount++;
    }

    private Dialogue GetRandomDialogue()
    {
        return _dialogues[Random.Range(0, _dialogues.Length)];
    }
}
