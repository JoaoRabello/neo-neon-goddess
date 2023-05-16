using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    private Dialogue _selectedDialogue;
    [NonSerialized] private GUIStyle _nodeStyle;

    [NonSerialized] private DialogueNode _draggingNode;
    [NonSerialized] private DialogueNode _creatingNode;
    [NonSerialized] private DialogueNode _deletingNode;
    [NonSerialized] private DialogueNode _linkingParentNode;
    [NonSerialized] private Vector2 _draggingOffset;
    [NonSerialized] private Vector2 _draggingCanvasOffset;
    [NonSerialized] private bool _isDraggingCanvas;

    private Vector2 _scrollPosition;

    private const float CANVAS_SIZE = 4000;
    private const float BACKGROUND_SIZE = 50;

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

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
        var backgroundTexture = Resources.Load("background") as Texture2D;
        var textureCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
        GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);
        
        foreach (var node in _selectedDialogue.GetAllNodes())
        {
            DrawConnections(node);
        }
        foreach (var node in _selectedDialogue.GetAllNodes())
        {
            OnDrawNode(node);
        }
        
        EditorGUILayout.EndScrollView();

        if (_creatingNode is not null)
        {
            Undo.RecordObject(_selectedDialogue, "Added Dialogue Node");
            _selectedDialogue.CreateNode(_creatingNode);
            _creatingNode = null;
        }
        if (_deletingNode is not null)
        {
            Undo.RecordObject(_selectedDialogue, "Deleted Dialogue Node");
            _selectedDialogue.DeleteNode(_deletingNode);
            _deletingNode = null;
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
            _draggingNode = GetNodeAtPosition(Event.current.mousePosition + _scrollPosition);

            if (_draggingNode != null)
            {
                _draggingOffset = _draggingNode.Rect.position - Event.current.mousePosition;
            }
            else
            {
                _isDraggingCanvas = true;
                _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
            }

        }
        else if (Event.current.type == EventType.MouseDrag && _draggingNode is not null)
        {
            Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");
            _draggingNode.Rect.position = Event.current.mousePosition + _draggingOffset;
            GUI.changed = true;
        }
        else if (Event.current.type == EventType.MouseDrag && _isDraggingCanvas)
        {
            _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;

            GUI.changed = true;
        }
        else if (Event.current.type == EventType.MouseUp && _draggingNode is not null)
        {
            _draggingNode = null;
        }
        else if (Event.current.type == EventType.MouseUp && _isDraggingCanvas)
        {
            _isDraggingCanvas = false;
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

        var newText = EditorGUILayout.TextField(node.Text);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_selectedDialogue, "Update Dialogue Text");
            node.Text = newText;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("x"))
        {
            _deletingNode = node;
        }
        if (GUILayout.Button("+"))
        {
            _creatingNode = node;
        }

        DrawLinkButtons(node);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawLinkButtons(DialogueNode node)
    {
        if (_linkingParentNode is null)
        {
            if (GUILayout.Button("link"))
            {
                _linkingParentNode = node;
            }
        }
        else if (_linkingParentNode == node)
        {
            if (GUILayout.Button("cancel"))
            {
                _linkingParentNode = null;
            }
        }
        else if (_linkingParentNode.Children.Contains(node.Id))
        {
            if (GUILayout.Button("unlink"))
            {
                Undo.RecordObject(_selectedDialogue, "Remove Dialogue Link");
                _linkingParentNode.Children.Remove(node.Id);
                _linkingParentNode = null;
            }
        }
        else
        {
            if (GUILayout.Button("child"))
            {
                Undo.RecordObject(_selectedDialogue, "Add Dialogue Link");
                _linkingParentNode.Children.Add(node.Id);
                _linkingParentNode = null;
            }
        }
    }
}
