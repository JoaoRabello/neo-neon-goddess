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
    public string[] Children;
    public Rect Rect = new Rect(0, 0, 200, 100);
}
