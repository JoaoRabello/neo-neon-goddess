using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Item", menuName = "ScriptableObjects/Inventory/Heal Item")]
public class HealItem : Item
{
    public enum HealType
    {
        Physical,
        Mental,
        Both
    }

    public HealType Type;
    public int Amount;
}
