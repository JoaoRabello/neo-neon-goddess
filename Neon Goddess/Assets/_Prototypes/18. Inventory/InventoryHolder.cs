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
        _myRenderer.RenderInventory(_inventory.GetItemsWithoutAmmoAsList(), _inventory.GetAmmoAsList());
    }

    public void CloseInventory()
    {
        _myRenderer.HideInventory();
    }

    public bool TryAddItem(Item item, int amount = 1)
    {
        return _inventory.TryAddItem(item, amount);
    }
}
