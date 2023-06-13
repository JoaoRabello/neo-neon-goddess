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
    public bool isOnRange = false;
   
    
    private void Awake()
    {
        doorShaderScript = referenceDoor.GetComponent<DoorShaderScript>();
        doorShaderScript.doorStatus = false; }
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
        if (!isOnRange) return;
        if (!PlayerInventoryObserver.Instance.TryConsumeItem(_key)) return;
        doorShaderScript.doorStatus = true;
                        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOnRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOnRange = false;

        }
    }

 
}
