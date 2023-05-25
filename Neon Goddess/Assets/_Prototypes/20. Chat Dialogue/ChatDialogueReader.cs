using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatDialogueReader : MonoBehaviour
{
    public static ChatDialogueReader Instance;
    
    [SerializeField] private GameObject _dialogueVisualContent;
    [SerializeField] private TMP_Text _dialogueLabel;

    public Action DialogueEnded;
    
    private InputActions _inputActions;
    private Dialogue _currentDialogue;
    private DialogueNode _currentDialogueNode;

    private bool _onDialogue;

    private void Awake()
    {
        Instance = this;
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
        if (!_onDialogue) return;
        
        if (_currentDialogueNode.Children.Count <= 0)
        {
            _onDialogue = false;
            
            _dialogueVisualContent.SetActive(false);
            DialogueEnded?.Invoke();
            return;
        }
        
        _currentDialogueNode = _currentDialogue.GetAllChildren(_currentDialogueNode).ToArray()[0];
        SetDialogueText();
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        _onDialogue = true;
        _currentDialogue = dialogue;
        _currentDialogueNode = dialogue.GetRootNode();
        
        _dialogueVisualContent.SetActive(true);
        SetDialogueText();
    }

    private void SetDialogueText()
    {
        _dialogueLabel.SetText(_currentDialogueNode.Text);
    }
}
