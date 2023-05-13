using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues/Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] private List<DialogueNode> _nodes = new List<DialogueNode>();
    private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
    private void Awake()
    {
        if (_nodes.Count > 0) return;

        var rootNode = new DialogueNode();

        rootNode.Id = Guid.NewGuid().ToString();
        _nodes.Add(rootNode);
        OnValidate();
    }
#endif

    private void OnValidate()
    {
        _nodeLookup.Clear();

        foreach (var node in GetAllNodes())
        {
            _nodeLookup[node.Id] = node;
        }
    }

    public IEnumerable<DialogueNode> GetAllNodes()
    {
        return _nodes;
    }

    public DialogueNode GetRootNode()
    {
        return _nodes[0];
    }

    public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
    {
        foreach (var childId in parentNode.Children)
        {
            if(!_nodeLookup.ContainsKey(childId)) continue;
            
            yield return _nodeLookup[childId];
        }
    }

    public void CreateNode(DialogueNode parentNode)
    {
        var newNode = new DialogueNode
        {
            Id = Guid.NewGuid().ToString()
        };
        parentNode.Children.Add(newNode.Id);
        _nodes.Add(newNode);
        
        OnValidate();
    }
}
