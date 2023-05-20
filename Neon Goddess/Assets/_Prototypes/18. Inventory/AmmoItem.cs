using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "ScriptableObjects/Inventory/Ammo Item")]
public class AmmoItem : Item
{
    public enum AmmoType
    {
        Paralysis,
        Override,
        Conversion,
        Control
    }

    public AmmoType Type;
    public int Damage;
}
