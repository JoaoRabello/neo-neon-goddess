using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Player;
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
    
    private Dialogue _currentDialogue;
    private DialogueNode _currentDialogueNode;

    private bool _onDialogue;
    private bool _firstInteraction = true;
    private bool _choosing;
    private int _choiceIndex;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
    
    private void OnEnable()
    {
        PlayerInputReader.Instance.InteractPerformed += OnInteractPerformed;
        PlayerInputReader.Instance.MovementPerformed += ChangeIndex;
    }

    private void OnDisable()
    {
        PlayerInputReader.Instance.InteractPerformed -= OnInteractPerformed;
        PlayerInputReader.Instance.MovementPerformed -= ChangeIndex;
    }

    private void OnInteractPerformed()
    {
        if (!_onDialogue) return;
        if (_firstInteraction)
        {
            _firstInteraction = false;
            return;
        }

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

    private void ChangeIndex(Vector2 input)
    {
        if (!_choosing) return;
        
        if (input.y < 0)
        {
            _choiceIndex++;
        }
        else if (input.y > 0)
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
            _firstInteraction = true;
            
            _dialogueVisualContent.SetActive(false);
            DialogueEnded?.Invoke();
            
            PlayerStateObserver.Instance.OnDialogueEnd();
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

        PlayerStateObserver.Instance.OnDialogueStart();
        
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
