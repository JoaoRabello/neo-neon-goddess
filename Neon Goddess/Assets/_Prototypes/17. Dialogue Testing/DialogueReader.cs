using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueReader : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueLabel;
    [SerializeField] private TMP_Text _characterNameLabel;
    [SerializeField] private Dialogue _testDialogue;

    private DialogueNode _currentNode;
    
    void Start()
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

    public void Next()
    {
        if(!HasNext()) return;
        
        var children = _testDialogue.GetAllChildren(_currentNode).ToArray();
        _currentNode = children[0];
        
        UpdateLabels(_currentNode);
    }

    private bool HasNext()
    {
        return _testDialogue.GetAllChildren(_currentNode).Any();
    }
}
