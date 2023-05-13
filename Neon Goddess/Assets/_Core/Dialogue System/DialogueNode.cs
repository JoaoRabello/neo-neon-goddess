using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DialogueNode
{
    public string Id;
    public string Text;
    public List<String> Children = new List<string>();
    public Rect Rect = new Rect(50, 150, 200, 100);
}
