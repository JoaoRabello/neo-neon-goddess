using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    private Dialogue _selectedDialogue;
    private GUIStyle _nodeStyle;

    private DialogueNode _draggingNode = null;
    private Vector2 _draggingOffset;

    [MenuItem("Dialogue Designer/Open Editor")]
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(DialogueEditor), false, "Dialogue Designer");
    }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
        
        if (dialogue == null) return false;
        
        ShowEditorWindow();
        return true;
    }

    private void OnEnable()
    {
        _nodeStyle = new GUIStyle();
        _nodeStyle.normal.textColor = Color.white;
        _nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
        _nodeStyle.padding = new RectOffset(20, 20, 20, 20);
        _nodeStyle.border = new RectOffset(12, 12, 12, 12);
    }

    private void OnSelectionChange()
    {
        SelectDialogueAsset();
    }

    private void SelectDialogueAsset()
    {
        if (Selection.activeObject is not Dialogue activeObjectAsDialogue) return;

        _selectedDialogue = activeObjectAsDialogue;
        Repaint();
    }

    private void OnGUI()
    {
        if (_selectedDialogue is null)
        {
            EditorGUILayout.LabelField("No Dialogue Selected");
            return;
        }

        ProcessEvents();
        foreach (var node in _selectedDialogue.GetAllNodes())
        {
            DrawConnections(node);
        }
        foreach (var node in _selectedDialogue.GetAllNodes())
        {
            OnDrawNode(node);
        }
    }

    private void DrawConnections(DialogueNode node)
    {
        var startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);

        foreach (var childNode in _selectedDialogue.GetAllChildren(node))
        {
            var endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);
            var controlPointOffset = endPosition - startPosition;
            controlPointOffset.y = 0;
            controlPointOffset.x *= 0.8f;
            
            Handles.DrawBezier(startPosition, endPosition, 
                startPosition + controlPointOffset, endPosition - controlPointOffset, 
                Color.white, null, 4f);
        }
    }

    private void ProcessEvents()
    {
        if (Event.current.type == EventType.MouseDown && _draggingNode is null)
        {
            _draggingNode = GetNodeAtPosition(Event.current.mousePosition);

            if (_draggingNode is null) return;

            _draggingOffset = _draggingNode.Rect.position - Event.current.mousePosition;
        }
        else if (Event.current.type == EventType.MouseDrag && _draggingNode is not null)
        {
            Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");
            _draggingNode.Rect.position = Event.current.mousePosition + _draggingOffset;
            GUI.changed = true;
        }
        else if (Event.current.type == EventType.MouseUp && _draggingNode is not null)
        {
            _draggingNode = null;
        }
    }

    private DialogueNode GetNodeAtPosition(Vector2 currentMousePosition)
    {
        DialogueNode foundNode = null;
        foreach (var node in _selectedDialogue.GetAllNodes())
        {
            if (node.Rect.Contains(currentMousePosition))
            {
                foundNode =  node;
            }
        }

        return foundNode;
    }

    private void OnDrawNode(DialogueNode node)
    {
        GUILayout.BeginArea(node.Rect, _nodeStyle);
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Node: ");
        var newText = EditorGUILayout.TextField(node.Text);
        var newId = EditorGUILayout.TextField(node.Id);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_selectedDialogue, "Update Dialogue Text");
            node.Text = newText;
            node.Id = newId;
        }
        
        GUILayout.EndArea();
    }
}
