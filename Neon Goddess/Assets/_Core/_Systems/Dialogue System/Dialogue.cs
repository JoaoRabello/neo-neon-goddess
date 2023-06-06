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

#if UNITY_EDITOR
    public void CreateNode(DialogueNode parentNode)
    {
        var newNode = BuildNode(parentNode);
        Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
        Undo.RecordObject(this, "Added Dialogue Node");

        AddNode(newNode);
    }

    private static DialogueNode BuildNode(DialogueNode parentNode)
    {
        var newNode = CreateInstance<DialogueNode>();
        newNode.name = Guid.NewGuid().ToString();

        if (parentNode != null)
        {
            parentNode.AddChild(newNode.name);
            newNode.SetPosition(parentNode.NodeRect.position + new Vector2(250, 0));
        }

        return newNode;
    }
    
    private void AddNode(DialogueNode newNode)
    {
        _nodes.Add(newNode);
        OnValidate();
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        Undo.RecordObject(this, "Deleted Dialogue Node");

        _nodes.Remove(nodeToDelete);
        OnValidate();
        ClearDanglingChildren(nodeToDelete);
        
        Undo.DestroyObjectImmediate(nodeToDelete);
    }

    private void ClearDanglingChildren(DialogueNode nodeToDelete)
    {
        foreach (var node in GetAllNodes())
        {
            node.RemoveChild(nodeToDelete.name);
        }
    }
#endif

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (_nodes.Count == 0)
        {
            var newNode = BuildNode(null);
            newNode.SetAsRootNode();
            AddNode(newNode);
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
#endif
    }

    public void OnAfterDeserialize()
    {
    }
}
