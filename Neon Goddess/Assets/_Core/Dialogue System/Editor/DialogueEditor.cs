using System;
using UnityEditor;
using UnityEditor.Callbacks;

public class DialogueEditor : EditorWindow
{
    private Dialogue _selectedDialogue;
    
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
    
    private void OnSelectionChange()
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
        
        EditorGUILayout.LabelField(_selectedDialogue.name);
    }
}
