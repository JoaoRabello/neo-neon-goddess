using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueNode[] nodes;
}
