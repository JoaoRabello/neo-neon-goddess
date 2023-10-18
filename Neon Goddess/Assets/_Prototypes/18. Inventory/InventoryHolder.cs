using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryHolder : MonoBehaviour
{
    [SerializeField] protected Inventory _inventory;
    [SerializeField] protected InventoryRenderer _myRenderer;
    
    [SerializeField] protected bool _hasItemUsage;

    public Action<Item> OnItemAdded;

    private void OnEnable()
    {
        if(!_hasItemUsage) return;
        _myRenderer.ItemConsumed += ConsumeItem;
    }

    private void OnDisable()
    {
        if(!_hasItemUsage) return;
        _myRenderer.ItemConsumed -= ConsumeItem;
    }

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
        var couldAdd = _inventory.TryAddItem(item, amount);
        if (couldAdd)
        {
            OnItemAdded?.Invoke(item);
        }
        return couldAdd;
    }
    
    private void ConsumeItem(Item item)
    {
        _inventory.TryRemoveItem(item);
        _myRenderer.RenderInventory(_inventory.GetItemsWithoutAmmoAsList(), _inventory.GetAmmoAsList());
    }

    private void ConsumeItemOnBackground(Item item)
    {
        _inventory.TryRemoveItem(item);
    }

    public bool TryConsumeItem(Item item)
    {
        if (!_inventory.HasItem(item)) return false;
        
        ConsumeItemOnBackground(item);
        return true;
    }
}
