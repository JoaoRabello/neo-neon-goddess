using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private KeyItem _key;
    
    private void OnEnable()
    {
        PlayerInputReader.Instance.InteractPerformed += TryOpen;
    }
    
    private void OnDisable()
    {
        PlayerInputReader.Instance.InteractPerformed -= TryOpen;
    }

    private void TryOpen()
    {
        if (!PlayerInventoryObserver.Instance.TryConsumeItem(_key)) return;
        
        //TODO: Make door open
        gameObject.SetActive(false);
    }
}
