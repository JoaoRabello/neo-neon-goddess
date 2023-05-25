using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCChatInteraction : MonoBehaviour
{
    [SerializeField] private Dialogue _dialogue;
    
    private InputActions _inputActions;

    private bool _isNextToNPC;

    private void Awake()
    {
        _inputActions = new InputActions();
        ChatDialogueReader.Instance.DialogueEnded += OnDialogueEnded;
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

    private void OnDialogueEnded()
    {
        _inputActions.Enable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!_isNextToNPC) return;
        
        ChatDialogueReader.Instance.PlayDialogue(_dialogue);
        _inputActions.Disable();
    }

    public void SetIsNextToNPC(bool value)
    {
        _isNextToNPC = value;
    }
}
