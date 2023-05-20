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
    }

    private void RenderItems(List<KeyValuePair<Item, int>> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            _renderObjects[i].Setup(items[i].Key, items[i].Value);
            _renderObjects[i].ButtonClicked += OnItemButtonClicked;
        }
    }
    
    private void RenderDescription(Item item)
    {
        if(!_hasDescription) return;
        
        _itemNameLabel.SetText(item.Name);
        _itemDescriptionLabel.SetText(item.Description);
    }
    
    private void OnItemButtonClicked(Item item)
    {
        RenderDescription(item);
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
