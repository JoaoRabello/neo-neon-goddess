using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventoryHolder : InventoryHolder
{
    [Header("Player Info")]
    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private InventoryRenderer _playerInventoryRenderer;

    public override void OpenInventory()
    {
        base.OpenInventory();
        
        _playerInventoryRenderer.RenderInventory(_playerInventory.GetItemsAsList());
    }
}
