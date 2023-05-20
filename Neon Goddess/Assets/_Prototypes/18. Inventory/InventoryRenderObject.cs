using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRenderObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _amountLabel;
    [SerializeField] private Image _iconImage;
    
    private KeyValuePair<Item, int> _item;

    public Action<KeyValuePair<Item, int>> ButtonClicked;
    
    public void ResetInfo()
    {
        _item = default;
        
        _amountLabel.gameObject.SetActive(false);
        _iconImage.gameObject.SetActive(false);
    }
    
    public void Setup(KeyValuePair<Item, int> item)
    {
        _item = item;
        
        if (item.Value > 1)
        {
            _amountLabel.gameObject.SetActive(true);
            _amountLabel.SetText($"x{item.Value}");
        }
        
        _iconImage.gameObject.SetActive(true);
        _iconImage.sprite = item.Key.Icon;
    }

    public void ClickButton()
    {
        ButtonClicked?.Invoke(_item);
    }
}
