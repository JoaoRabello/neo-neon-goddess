using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Item _testItem;

    private void Start()
    {
        TryAddItem(_testItem);
        TryAddItem(_testItem);
        TryAddItem(_testItem);
        TryAddItem(_testItem);
    }

    public bool TryAddItem(Item item, int amount = 1)
    {
        if (!_inventory.TryAddItem(item, amount))
        {
            Debug.Log("Couldn't Add Item");
            return false;
        }

        Debug.Log($"Added x{amount} {item.Name}");
        return true;
    }
}
