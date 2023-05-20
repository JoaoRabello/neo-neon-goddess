using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private List<InventoryRenderObject> _renderObjects = new List<InventoryRenderObject>();

    [Header("Item Description")] 
    [SerializeField] private bool _hasDescription;
    [SerializeField] private TMP_Text _itemNameLabel;
    [SerializeField] private TMP_Text _itemDescriptionLabel;
    
    [Header("Item Transfer")] 
    [SerializeField] private bool _hasTransfer;
    [SerializeField] private GameObject _transferItemContent;
    private KeyValuePair<Item, int> _selectedItemToTransfer;
    
    public Action<KeyValuePair<Item, int>> TransferButtonClicked;
    
    public void RenderInventory(List<KeyValuePair<Item, int>> items)
    {
        if(_content) _content.SetActive(true);
        ResetRenderObjects();

        RenderItems(items);
    }

    public void HideInventory()
    {
        ResetRenderObjects();
        ResetDescription();

        if(_content) _content.SetActive(false);
        if(_hasDescription) _transferItemContent.SetActive(false);
    }

    private void RenderItems(List<KeyValuePair<Item, int>> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            _renderObjects[i].Setup(items[i]);
            _renderObjects[i].ButtonClicked += OnItemButtonClicked;
        }
    }
    
    private void RenderDescription(Item item)
    {
        if(!_hasDescription) return;
        
        _itemNameLabel.SetText(item.Name);
        _itemDescriptionLabel.SetText(item.Description);
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
    }

    private void ResetDescription()
    {
        if(!_hasDescription) return;
        
        _itemNameLabel.SetText("");
        _itemDescriptionLabel.SetText("");
    }
}
