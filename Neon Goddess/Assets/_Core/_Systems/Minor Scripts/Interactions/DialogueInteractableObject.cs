using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class DialogueInteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue _dialogue;
    
    public void Interact()
    {
        ChatDialogueReader.Instance.PlayDialogue(_dialogue);
    }
}
