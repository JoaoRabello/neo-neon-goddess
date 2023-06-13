using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class doorOpenByCode : MonoBehaviour
{
    public DoorShaderScript doorShaderScript;
    public TMP_InputField uiFieldCode;
    public string password;
    private string triedPassword;
    public GameObject referenceDoor;
    public bool isOnRange = false;
    

    // Start is called before the first frame update
    private InputActions _inputActions;

    private void Start()
    {
      
    }
    private void Awake()
    {
        _inputActions = new InputActions();
        doorShaderScript = referenceDoor.GetComponent<DoorShaderScript>();

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
                
        else if (triedPassword == password && doorShaderScript.doorStatus == false)
        {
            
            uiFieldCode.gameObject.SetActive(false);
            doorShaderScript.doorStatus = true;
            
            
        }

        
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
