using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private List<InventoryRenderObject> _renderObjects = new List<InventoryRenderObject>();

    public void RenderInventory(List<KeyValuePair<Item, int>> items)
    {
        _content.SetActive(true);
        ResetRenderObjects();

        RenderItems(items);
    }

    public void HideInventory()
    {
        ResetRenderObjects();

        _content.SetActive(false);
    }

    private void RenderItems(List<KeyValuePair<Item, int>> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            _renderObjects[i].Setup(items[i].Key, items[i].Value);
        }
    }

    private void ResetRenderObjects()
    {
        foreach (var renderObject in _renderObjects)
        {
            renderObject.ResetInfo();
        }
    }
}
