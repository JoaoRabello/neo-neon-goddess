using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private KeyItem _key;
    public GameObject referenceDoor;
    public bool doorStatus = false;
    public bool isOnRange = false;
    public GameObject doorTrigger;
    public Renderer doorShaderRenderer;
    public float doorEmissionIntensity;
    public Light doorLight;
    public Light doorLight2;
    public Renderer doorStatusRenderer;
    
    private void Awake()
    { doorStatus = false; }
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
        doorTrigger.SetActive(true);
        doorStatus = true;
                        
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

    void DoorUnlocked()
    {
        doorShaderRenderer.materials[1].SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.0f));
        doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(0.0f, 4342.935f, 0.0f, 1.0f));
        doorLight.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        doorLight2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        doorStatusRenderer.materials[2].SetInt("_Status", 1);
    }

    void DoorLocked()
    {
        doorShaderRenderer.materials[1].SetColor("_Color", new Color(1.0f, 0.0f, 0.0f, 0.0f));
        doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(4342.935f, 0.0f, 0.0f, 1.0f));
        doorLight.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        doorLight2.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        doorStatusRenderer.materials[2].SetInt("_Status", 0);
    }
    private void Update()
    {
        switch (doorStatus)
        {
            case true:
                DoorUnlocked();
                break;
            case false:
                DoorLocked();
                break;
        }
    }
}
