using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogues/Character")]
public class DialogueCharacter : ScriptableObject
{
    public string CharacterName;
    public Sprite CharacterSprite;
    public Color CharacterNodeColor;
}
