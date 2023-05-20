using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryHolder : MonoBehaviour
{
    [SerializeField] protected Inventory _inventory;
    [SerializeField] protected InventoryRenderer _myRenderer;

    public virtual void OpenInventory()
    {
        _myRenderer.RenderInventory(_inventory.GetItemsAsList());
    }

    public void CloseInventory()
    {
        _myRenderer.HideInventory();
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
