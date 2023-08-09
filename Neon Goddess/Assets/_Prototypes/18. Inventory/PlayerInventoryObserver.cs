using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryObserver : MonoBehaviour
{
    public static PlayerInventoryObserver Instance;
    [SerializeField] private InventoryHolder _playerInventoryHolder;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    public bool TryConsumeItem(Item item)
    {
        return _playerInventoryHolder.TryConsumeItem(item);
    }
}
