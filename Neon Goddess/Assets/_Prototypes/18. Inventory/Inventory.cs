using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "ScriptableObjects/Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public int Capacity;
    private Dictionary<Item, int> _itemDictionary = new Dictionary<Item, int>();

    public bool HasItem(Item item)
    {
        return _itemDictionary.ContainsKey(item) && _itemDictionary[item] > 0;
    }
    
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
    
    public bool TryRemoveItem(Item item, int amount = 1)
    {
        if (!_itemDictionary.ContainsKey(item)) return false;

        _itemDictionary[item] -= amount;
        
        if (_itemDictionary[item] <= 0)
        {
            _itemDictionary.Remove(item);
        }

        return true;
    }

    public List<KeyValuePair<Item, int>> GetItemsWithoutAmmoAsList()
    {
        return _itemDictionary.Where(pair => pair.Key.GetType() != typeof(AmmoItem)).ToList();
    }
    
    public List<KeyValuePair<Item, int>> GetAmmoAsList()
    {
        return _itemDictionary.Where(pair => pair.Key.GetType() == typeof(AmmoItem)).ToList();
    }
}
