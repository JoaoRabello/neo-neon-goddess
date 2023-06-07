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
        return _inventory.TryAddItem(item, amount);
    }
    
    private void ConsumeItem(Item item)
    {
        _inventory.TryRemoveItem(item);
        _myRenderer.RenderInventory(_inventory.GetItemsWithoutAmmoAsList(), _inventory.GetAmmoAsList());
    }
}