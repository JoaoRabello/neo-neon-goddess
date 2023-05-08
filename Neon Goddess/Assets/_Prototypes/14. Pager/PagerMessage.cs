using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New PagerMessage", menuName = "Prototype/Pager/Message")]
public class PagerMessage : ScriptableObject
{
    public int ID;
    public int SenderFrequency;
    public string Text;
    public bool Read;
}
