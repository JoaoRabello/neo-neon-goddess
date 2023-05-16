using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues/Dialogue")]
public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<DialogueNode> _nodes = new List<DialogueNode>();
    private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

    private void OnValidate()
    {
        _nodeLookup.Clear();

        foreach (var node in GetAllNodes())
        {
            _nodeLookup[node.name] = node;
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
        var newNode = CreateInstance<DialogueNode>();
        newNode.name = Guid.NewGuid().ToString();
        
        Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");

        if (parentNode != null)
        {
            parentNode.Children.Add(newNode.name);
        }
        _nodes.Add(newNode);
        OnValidate();
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        _nodes.Remove(nodeToDelete);
        OnValidate();
        ClearDanglingChildren(nodeToDelete);
        
        Undo.DestroyObjectImmediate(nodeToDelete);
    }

    private void ClearDanglingChildren(DialogueNode nodeToDelete)
    {
        foreach (var node in GetAllNodes())
        {
            node.Children.Remove(nodeToDelete.name);
        }
    }

    public void OnBeforeSerialize()
    {
        if (_nodes.Count == 0)
        {
            CreateNode(null);
        }
        
        if (AssetDatabase.GetAssetPath(this) != "")
        {
            foreach (var node in GetAllNodes())
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
        }
    }

    public void OnAfterDeserialize()
    {
        throw new NotImplementedException();
    }
}
