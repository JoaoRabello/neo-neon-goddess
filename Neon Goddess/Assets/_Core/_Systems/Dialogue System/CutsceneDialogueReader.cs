using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CutsceneDialogueReader : MonoBehaviour
{
    public static CutsceneDialogueReader Instance;
    
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private TimelineAsset _test;
    [SerializeField] private GameObject _dialogueVisualContent;
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
    private int _choiceIndex;
    private bool _isTypewriting;
    
    private double _timeToSkip;
    private bool _isPaused;
    private bool _canPassDialogue;

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

    public void PassCurrentDialogue()
    {
        if (!_onDialogue) return;

        if (_choosing)
        {
            _choosing = false;
            
            ClearOptions();

            if(_currentDialogueNode.Children.Count > 0) _currentDialogueNode = _currentDialogue.GetAllChildren(_currentDialogueNode).ToArray()[_choiceIndex];
            PlayNextNode();
            
            return;
        }

        _canPassDialogue = true;
        PlayNextNode();
    }

    private void OnInteractPerformed()
    {
        if (!_onDialogue) return;
        if(!_canPassDialogue) return;
        // if (_firstInteraction)
        // {
        //     _firstInteraction = false;
        //     return;
        // }

        if (_choosing)
        {
            _choosing = false;
            
            ClearOptions();

            if(_currentDialogueNode.Children.Count > 0) _currentDialogueNode = _currentDialogue.GetAllChildren(_currentDialogueNode).ToArray()[_choiceIndex];
            PlayNextNode();
            
            return;
        }

        if(_isTypewriting)
        {
            _isTypewriting = false;

            var timeToSkip = _playableDirector.time + _dialogueLabel.text.Length * _typeWritingCharacterAppearTime;
            _playableDirector.time = timeToSkip;
            
            StopAllCoroutines();
            _dialogueLabel.SetText(_currentDialogueNode.Text);

            _playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            _isPaused = true;
        }
        else
        {
            if (_isPaused)
            {
                ClearText();
                _playableDirector.time = _playableDirector.time;
                _playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                _isPaused = false;
                _canPassDialogue = false;
            }
            else
            {
                //TODO: Pular para o tempo previsto após a conclusão do diálogo. Usar um calculo de letras restantes * tempo de typewriting
                // _playableDirector.time = _timeToSkip;
            }
        }
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
            
            if(PlayerStateObserver.Instance != null) PlayerStateObserver.Instance.OnDialogueEnd();
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
        if (dialogue == null)
        {
            Debug.Log($"[ChatDialogueReader] PlayDialogue | Não há dialogo para tocar. Você tem certeza de que colocou o arquivo do diálogo no lugar certo?");
            return;
        }
        
        _onDialogue = true;
        _canPassDialogue = true;
        _currentDialogue = dialogue;
        dialogue.ResetNodeLookup();
        _currentDialogueNode = dialogue.GetRootNode();

        if(PlayerStateObserver.Instance != null) PlayerStateObserver.Instance.OnDialogueStart();
        
        _dialogueVisualContent.SetActive(true);
        SetDialogueText();
        
        SetTimeToSkip();
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

    private void SelectOption(DialogueNode node)
    {
        ClearOptions();

        _choosing = false;
        _currentDialogueNode = node;
        
        PlayNextNode();
    }

    private void ClearText()
    {
        _dialogueLabel.SetText("");
    }

    private void SetDialogueText()
    {
        StopAllCoroutines();
        StartCoroutine(PlayTypewrite(_currentDialogueNode.Text));
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
        _dialogueLabel.SetText(text);
        _isTypewriting = false;
        
        _playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        _isPaused = true;
        SetTimeToSkip();
    }

    public void SetTimeToSkip()
    {
        var allTracks = _test.GetOutputTracks();
        foreach (var track in allTracks)
        {
            if(track.name != "Signal Track") continue;

            double newTimeToSkip = 1000;
            
            var markers = track.GetMarkers();
            
            foreach (var marker in markers)
            {
                if (_timeToSkip < marker.time && marker.time <= newTimeToSkip)
                {
                    newTimeToSkip = marker.time;
                }
            }
            
            _timeToSkip = newTimeToSkip;
            break;
        }
    }
}
