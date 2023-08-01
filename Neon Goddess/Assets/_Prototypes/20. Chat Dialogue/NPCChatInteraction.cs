using System.Collections;
using System.Collections.Generic;
using Inputs;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCChatInteraction : MonoBehaviour
{
    [SerializeField] private Dialogue _dialogue;
    
    private bool _isNextToNPC;

    private void OnEnable()
    {
        PlayerInputReader.Instance.InteractPerformed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        PlayerInputReader.Instance.InteractPerformed -= OnInteractPerformed;
    }

    private void OnDialogueEnded()
    {
        PlayerStateObserver.Instance.OnDialogueEnd();
    }

    private void OnInteractPerformed()
    {
        if (!_isNextToNPC) return;
        
        ChatDialogueReader.Instance.PlayDialogue(_dialogue);
        // _inputActions.Disable();
    }

    public void SetIsNextToNPC(bool value)
    {
        _isNextToNPC = value;
    }
}
