using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventoryHolder : InventoryHolder
{
    [Header("Player Info")]
    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private InventoryRenderer _playerInventoryRenderer;

    private void OnEnable()
    {
        _myRenderer.TransferButtonClicked += OnChestToPlayerTransferButtonClicked;
        _playerInventoryRenderer.TransferButtonClicked += OnPlayerToChestTransferButtonClicked;
    }
    
    private void OnDisable()
    {
        _myRenderer.TransferButtonClicked -= OnChestToPlayerTransferButtonClicked;
        _playerInventoryRenderer.TransferButtonClicked -= OnPlayerToChestTransferButtonClicked;
    }

    private void OnChestToPlayerTransferButtonClicked(KeyValuePair<Item, int> item)
    {
        _inventory.TryRemoveItem(item.Key, item.Value);
        _playerInventory.TryAddItem(item.Key, item.Value);
        
        _myRenderer.CloseTransferView();
        
        UpdateInventoriesRenderers();
    }

    private void OnPlayerToChestTransferButtonClicked(KeyValuePair<Item, int> item)
    {
        _playerInventory.TryRemoveItem(item.Key, item.Value);
        _inventory.TryAddItem(item.Key, item.Value);
        
        _playerInventoryRenderer.CloseTransferView();

        UpdateInventoriesRenderers();
    }

    private void UpdateInventoriesRenderers()
    {
        _playerInventoryRenderer.RenderInventory(_playerInventory.GetItemsAsList());
        _myRenderer.RenderInventory(_inventory.GetItemsAsList());
    }

    public override void OpenInventory()
    {
        base.OpenInventory();
        
        _playerInventoryRenderer.RenderInventory(_playerInventory.GetItemsAsList());
    }
}
