using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Inventory/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public Transform prefab;
    public float rotationSpeed;
    [Tooltip("Check if this item can be used within the inventory")]
    public bool IsUsable;
    [Tooltip("Check if this item can be consumed within the inventory")]
    public bool IsConsumable;
}
