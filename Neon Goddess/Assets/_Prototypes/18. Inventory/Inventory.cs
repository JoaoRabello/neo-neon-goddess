using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "ScriptableObjects/Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public int Capacity;
    private Dictionary<Item, int> _itemDictionary = new Dictionary<Item, int>();

    public bool TryAddItem(Item item, int amount = 1)
    {
        if (_itemDictionary.Count >= Capacity && !_itemDictionary.ContainsKey(item)) return false;
        
        if (!_itemDictionary.ContainsKey(item))
        {
            _itemDictionary[item] = 0;
        }

        _itemDictionary[item] += amount;

        return true;
    }
}
