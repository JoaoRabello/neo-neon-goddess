using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRenderObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _amountLabel;
    [SerializeField] private Image _iconImage;
    
    private Item _item;

    public void ResetInfo()
    {
        _item = null;
        
        _amountLabel.gameObject.SetActive(false);
        _iconImage.gameObject.SetActive(false);
    }
    
    public void Setup(Item item, int amount)
    {
        _item = item;
        
        if (amount > 1)
        {
            _amountLabel.gameObject.SetActive(true);
            _amountLabel.SetText($"x{amount}");
        }
        
        _iconImage.gameObject.SetActive(true);
        _iconImage.sprite = item.Icon;
    }
}
