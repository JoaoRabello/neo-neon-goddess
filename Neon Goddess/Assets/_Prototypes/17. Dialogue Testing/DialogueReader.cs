using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueReader : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueLabel;
    [SerializeField] private TMP_Text _characterNameLabel;
    [SerializeField] private Dialogue _testDialogue;
    [SerializeField] private Button _resetButton;
    [SerializeField] private List<Button> _optionButtons = new List<Button>();

    private DialogueNode _currentNode;
    private bool _isChoosing;
    
    void Start()
    {
        _resetButton.onClick.AddListener(ResetDialogue);
        SetRootNode();
    }

    private void SetRootNode()
    {
        var rootNode = _testDialogue.GetRootNode();
        _currentNode = rootNode;
        
        UpdateLabels(rootNode);
    }

    private void UpdateLabels(DialogueNode node)
    {
        _dialogueLabel.SetText(node.Text);
        _characterNameLabel.SetText(node.Character.CharacterName);
    }

    private void ShowResetButton()
    {
        _resetButton.gameObject.SetActive(true);
    }

    private void ResetDialogue()
    {
        _resetButton.gameObject.SetActive(false);
        ClearOptions();
        _isChoosing = false;
        SetRootNode();
    }

    public void Next()
    {
        if (!HasNext())
        {
            ShowResetButton();
            return;
        }
        if(_isChoosing) return;

        var children = _testDialogue.GetAllChildren(_currentNode).ToArray();
        _currentNode = children[0];
        UpdateLabels(_currentNode);
        
        if (HasOptions())
        {
            _isChoosing = true;
        }

        if (_isChoosing)
        {
            var options = _testDialogue.GetAllChildren(_currentNode).ToArray();

            for (int i = 0; i < _optionButtons.Count; i++)
            {
                if(i >= options.Length) continue;

                var optionNode = options[i];
                
                _optionButtons[i].gameObject.SetActive(true);
                _optionButtons[i].GetComponentInChildren<TMP_Text>().SetText(optionNode.Text);
                _optionButtons[i].onClick.AddListener(() => SelectOption(optionNode));
            }
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

        _isChoosing = false;
        _currentNode = node;
        
        Next();
    }

    private bool HasNext()
    {
        return _testDialogue.GetAllChildren(_currentNode).Any();
    }

    private bool HasOptions()
    {
        return _testDialogue.GetAllChildren(_currentNode).Count() > 1;
    }
}
