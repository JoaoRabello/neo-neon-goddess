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
    [SerializeField] private List<TMP_Text> _dialogueOptionLabels = new List<TMP_Text>();

    public Action DialogueEnded;
    
    private InputActions _inputActions;
    private Dialogue _currentDialogue;
    private DialogueNode _currentDialogueNode;

    private bool _onDialogue;
    private bool _choosing;
    private int _choiceIndex;

    private void Awake()
    {
        Instance = this;
        _inputActions = new InputActions();
    }
    
    private void OnEnable()
    {
        _inputActions.Prototype.Interact.performed += OnInteractPerformed;
        _inputActions.Prototype.Movement.performed += ChangeIndex;
        
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Interact.performed -= OnInteractPerformed;
        _inputActions.Prototype.Movement.performed -= ChangeIndex;
        
        _inputActions.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!_onDialogue) return;

        if (_choosing)
        {
            _choosing = false;
            
            ResetDialogueOptions();

            if(_currentDialogueNode.Children.Count > 0) _currentDialogueNode = _currentDialogue.GetAllChildren(_currentDialogueNode).ToArray()[_choiceIndex];
            PlayNextNode();
            
            return;
        }
        
        PlayNextNode();
    }

    private void ChangeIndex(InputAction.CallbackContext context)
    {
        if (!_choosing) return;
        
        if (context.ReadValue<Vector2>().y < 0)
        {
            _choiceIndex++;
        }
        else if (context.ReadValue<Vector2>().y > 0)
        {
            _choiceIndex--;
        }

        _choiceIndex = Mathf.Clamp(_choiceIndex, 0, _currentDialogueNode.Children.Count - 1);
    }

    private void PlayNextNode()
    {
        if (_currentDialogueNode.Children.Count <= 0)
        {
            _onDialogue = false;
            
            _dialogueVisualContent.SetActive(false);
            DialogueEnded?.Invoke();
            return;
        }

        var children = _currentDialogue.GetAllChildren(_currentDialogueNode).ToArray();
        
        if (children.Length > 1)
        {
            ShowOptions(children);
        }
        else
        {
            _currentDialogueNode = children[0];
        }
        
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

    private void ShowOptions(DialogueNode[] nodes)
    {
        _choosing = true;
        
        for (int i = 0; i < _dialogueOptionLabels.Count; i++)
        {
            if (i >= nodes.Length)
            {
                _dialogueOptionLabels[i].gameObject.SetActive(false);
                continue;
            }
            
            _dialogueOptionLabels[i].gameObject.SetActive(true);
            _dialogueOptionLabels[i].SetText(nodes[i].Text);
        }
    }

    private void ResetDialogueOptions()
    {
        foreach (var label in _dialogueOptionLabels)
        {
            label.gameObject.SetActive(false);
        }
    }

    private void SetDialogueText()
    {
        _dialogueLabel.SetText(_currentDialogueNode.Text);
    }
}
