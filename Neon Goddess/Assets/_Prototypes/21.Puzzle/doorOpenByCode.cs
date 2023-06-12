using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class doorOpenByCode : MonoBehaviour
{
    public TMP_InputField uiFieldCode;
    public string password;
    private string triedPassword;
    public GameObject referenceDoor;
    public bool doorStatus = false;
    public bool isOnRange = false;
    public GameObject doorTrigger;
    public Renderer doorShaderRenderer;
    public float doorEmissionIntensity;
    public Light doorLight;
    public Light doorLight2;
    public Renderer doorStatusRenderer;

    // Start is called before the first frame update
    private InputActions _inputActions;

    private void Start()
    {
      
    }
    private void Awake()
    {
        _inputActions = new InputActions();
        

    }

    private void OnEnable()
    {
        _inputActions.Prototype.Interact.performed += OnInteractPerformed;

        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Interact.performed -= OnInteractPerformed;

        _inputActions.Disable();
    }

    private void OnDialogueEnded()
    {
        _inputActions.Enable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        triedPassword = uiFieldCode.text;

        if (triedPassword != password && isOnRange == true)
        {
            uiFieldCode.gameObject.SetActive(true);
        }
                
        else if (triedPassword == password && doorStatus == false)
        {
            
            uiFieldCode.gameObject.SetActive(false);
            doorStatus = true;
            //doorTrigger.gameObject.SetActive(true);
            
        }

        // codigo no fieldcode -> triedPassword
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
        doorTrigger.gameObject.SetActive(true);
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
