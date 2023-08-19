using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private KeyItem _key;
    public DoorShaderScript doorShaderScript;
    public GameObject referenceDoor;
    
    private void Awake()
    {
        doorShaderScript = referenceDoor.GetComponent<DoorShaderScript>();
        doorShaderScript.doorStatus = false; 
    }

    private void TryOpen()
    {
        if (!PlayerInventoryObserver.Instance.TryConsumeItem(_key)) return;
        doorShaderScript.doorStatus = true;
        gameObject.SetActive(false);
    }

    public void Interact()
    {
        TryOpen();
    }

    public IInteractable.InteractableType GetType()
    {
        return IInteractable.InteractableType.Door;
    }

    public bool HasInteractedOnce()
    {
        return false;
    }

    public bool IsLocked()
    {
        return !doorShaderScript.doorStatus;
    }
}
