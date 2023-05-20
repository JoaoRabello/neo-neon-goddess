using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private List<InventoryRenderObject> _renderObjects = new List<InventoryRenderObject>();

    [Header("Ammo")] 
    [SerializeField] private List<InventoryRenderObject> _ammoRenderObjects = new List<InventoryRenderObject>();
    
    [Header("Item Description")] 
    [SerializeField] private bool _hasDescription;
    [SerializeField] private TMP_Text _itemNameLabel;
    [SerializeField] private TMP_Text _itemDescriptionLabel;
    [SerializeField] private Button _useItemButton;
    
    [Header("Item Transfer")] 
    [SerializeField] private bool _hasTransfer;
    [SerializeField] private GameObject _transferItemContent;
    private KeyValuePair<Item, int> _selectedItemToTransfer;
    
    public Action<KeyValuePair<Item, int>> TransferButtonClicked;
    
    public void RenderInventory(List<KeyValuePair<Item, int>> items, List<KeyValuePair<Item, int>> ammo = default)
    {
        if(_content) _content.SetActive(true);
        ResetRenderObjects();

        RenderItems(items);
        if(ammo != default) RenderAmmo(ammo);
    }

    public void HideInventory()
    {
        ResetRenderObjects();
        ResetDescription();

        if(_content) _content.SetActive(false);
        if(_hasTransfer) _transferItemContent.SetActive(false);
    }

    private void RenderItems(List<KeyValuePair<Item, int>> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            _renderObjects[i].Setup(items[i]);
            _renderObjects[i].ButtonClicked += OnItemButtonClicked;
        }
    }

    private void RenderAmmo(List<KeyValuePair<Item, int>> ammo)
    {
        for (int i = 0; i < ammo.Count; i++)
        {
            _ammoRenderObjects[i].Setup(ammo[i]);
        }
    }
    
    private void RenderDescription(Item item)
    {
        if(!_hasDescription) return;
        
        _itemNameLabel.SetText(item.Name);
        _itemDescriptionLabel.SetText(item.Description);
        
        _useItemButton.gameObject.SetActive(item.IsUsable);
    }
    
    private void OnItemButtonClicked(KeyValuePair<Item, int> item)
    {
        RenderDescription(item.Key);
        
        if (!_hasTransfer) return;
        
        _selectedItemToTransfer = item;
        _transferItemContent.SetActive(true);
    }

    public void CloseTransferView()
    {
        _selectedItemToTransfer = default;
        _transferItemContent.SetActive(false);
    }

    public void OnTransferItemButtonClicked()
    {
        TransferButtonClicked?.Invoke(_selectedItemToTransfer);
    }

    private void ResetRenderObjects()
    {
        foreach (var renderObject in _renderObjects)
        {
            renderObject.ResetInfo();
            renderObject.ButtonClicked -= OnItemButtonClicked;
        }

        foreach (var ammoRenderObject in _ammoRenderObjects)
        {
            ammoRenderObject.ResetInfo();
        }
    }

    private void ResetDescription()
    {
        if(!_hasDescription) return;
        
        _itemNameLabel.SetText("");
        _itemDescriptionLabel.SetText("");
    }
}
