using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChatDialogueReader : MonoBehaviour
{
    public static ChatDialogueReader Instance;
    
    [SerializeField] private GameObject _dialogueVisualContent;
    [SerializeField] private GameObject _documentContent;
    [SerializeField] private GameObject _screenBackground;
    [SerializeField] private Image _documentImage;
    [SerializeField] private TMP_Text _documentLabel;
    [SerializeField] private TMP_Text _dialogueLabel;
    [SerializeField] private List<Button> _optionButtons = new List<Button>();
    [SerializeField] private List<TMP_Text> _dialogueOptionLabels = new List<TMP_Text>();
    
    [Header("Properties")]
    [SerializeField] private float _typeWritingCharacterAppearTime;

    public Action DialogueEnded;
    
    private Dialogue _currentDialogue;
    private DialogueNode _currentDialogueNode;

    private bool _onDialogue;
    private bool _firstInteraction = true;
    private bool _choosing;
    private bool _isTypewriting;
    private int _choiceIndex;

    private Camera _dialogueCamera;

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

        if (_isTypewriting)
        {
            SetDialogueTextWithoutTypewrite();
            _isTypewriting = false;
            return;
        }

        if (_choosing)
        {
            _choosing = false;
            
            ClearOptions();

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
            _documentContent.SetActive(false);
            DialogueEnded?.Invoke();
            
            PlayerStateObserver.Instance.OnDialogueEnd();
            return;
        }

        var children = _currentDialogue.GetAllChildren(_currentDialogueNode).ToArray();
        
        if (children.Length > 1)
        {
            ShowOptions(children);
            SetDialogueTextWithoutTypewrite();
        }
        else
        {
            _currentDialogueNode = children[0];

            RenderCurrentNode();
        }
    }

    public void PlayDialogue(Dialogue dialogue, Camera camera = null)
    {
        if (dialogue == null)
        {
            Debug.Log($"[ChatDialogueReader] PlayDialogue | Não há dialogo para tocar. Você tem certeza de que colocou o arquivo do diálogo no lugar certo?");
            return;
        }

        ClearDocument();

        _dialogueCamera = camera;
        
        _onDialogue = true;
        _currentDialogue = dialogue;
        dialogue.ResetNodeLookup();
        _currentDialogueNode = dialogue.GetRootNode();

        PlayerStateObserver.Instance.OnDialogueStart();
        
        RenderCurrentNode();
    }

    private void RenderCurrentNode()
    {
        ClearDialogueScreen();
        ClearDocument();
        ClearCameraScreen();

        switch (_currentDialogueNode.Type)
        {
            case DialogueNode.NodeType.Common:
                _dialogueVisualContent.SetActive(true);
                SetDialogueText();
                break;
            case DialogueNode.NodeType.Document:
                SetDocumentScreen();
                break;
            case DialogueNode.NodeType.Camera:
                if (_dialogueCamera == null) return;
                SetCameraScreen();
                break;
        }
    }

    private void ShowOptions(DialogueNode[] nodes)
    {
        _choosing = true;
        
        for (int i = 0; i < _dialogueOptionLabels.Count; i++)
        {
            if (i >= nodes.Length)
            {
                continue;
            }
            
            _dialogueOptionLabels[i].SetText(nodes[i].Text);
            var optionNode = nodes[i];
            _optionButtons[i].gameObject.SetActive(true);
            _optionButtons[i].onClick.AddListener(() => SelectOption(optionNode));
        }
    }
    
    private void ClearOptions()
    {
        foreach (var button in _optionButtons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }

    private void ClearDialogueScreen()
    {
        _dialogueVisualContent.SetActive(false);
    }

    private void ClearDocument()
    {
        _documentContent.SetActive(false);
    }
    
    private void ClearCameraScreen()
    {
        if (_dialogueCamera == null) return;
        
        _documentContent.SetActive(false);
        
        _screenBackground.SetActive(true);
        _dialogueCamera.gameObject.SetActive(false);
        CameraManager.Instance.TurnOnLastRoomCamera();
    }

    private void SelectOption(DialogueNode node)
    {
        ClearOptions();

        _choosing = false;
        _currentDialogueNode = node;
        
        PlayNextNode();
    }

    private void SetDocumentScreen()
    {
        _documentContent.SetActive(true);
        _documentLabel.gameObject.SetActive(true);
        _documentLabel.SetText(_currentDialogueNode.Text);
    }
    
    private void SetCameraScreen()
    {
        _documentContent.SetActive(true);
        
        CameraManager.Instance.TurnOffRoomCamera();
        _screenBackground.SetActive(false);
        _documentLabel.gameObject.SetActive(false);
        _dialogueCamera.gameObject.SetActive(true);
    }

    private void SetDialogueText()
    {
        StopAllCoroutines();
        StartCoroutine(PlayTypewrite(_currentDialogueNode.Text));
    }
    
    private void SetDialogueTextWithoutTypewrite()
    {
        StopAllCoroutines();
        _dialogueLabel.SetText(_currentDialogueNode.Text);
    }
    
    private IEnumerator PlayTypewrite(string text)
    {
        _isTypewriting = true;
        _dialogueLabel.SetText("");
        foreach (var character in text)
        {
            _dialogueLabel.SetText((_dialogueLabel.text + character));
            yield return new WaitForSeconds(_typeWritingCharacterAppearTime);
        }
        
        _isTypewriting = false;
    }
}
