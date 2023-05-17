using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DialogueNode : ScriptableObject
{
    [SerializeField] private string _text;
    [SerializeField] private List<String> _children = new List<string>();
    [SerializeField] private Rect _rect = new Rect(50, 150, 200, 100);

    public Rect NodeRect => _rect;
    public string Text => _text;
    public List<String> Children => _children;

#if UNITY_EDITOR
    public void SetPosition(Vector2 position)
    {
        Undo.RecordObject(this, "Move Dialogue Node");
        _rect.position = position;
        
        EditorUtility.SetDirty(this);
    }

    public void SetText(string text)
    {
        if(_text == text) return;
        
        Undo.RecordObject(this, "Update Dialogue Text");
        _text = text;
        
        EditorUtility.SetDirty(this);
    }

    public void AddChild(string id)
    {
        Undo.RecordObject(this, "Add Dialogue Link");
        _children.Add(id);
        EditorUtility.SetDirty(this);
    }
    
    public void RemoveChild(string id)
    {
        Undo.RecordObject(this, "Remove Dialogue Link");
        _children.Remove(id);
        EditorUtility.SetDirty(this);
    }
#endif
}
