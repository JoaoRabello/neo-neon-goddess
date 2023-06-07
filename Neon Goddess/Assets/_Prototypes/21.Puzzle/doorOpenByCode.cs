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
    public int doorOffSety;
    public int doorOffSetx;
    public int doorOffSetz;
    public bool doorStatus = false;
    public bool isOnRange = false;
    public GameObject doorTrigger;
    public Material doorShader;

    // Start is called before the first frame update
    private InputActions _inputActions;

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
            //referenceDoor.transform.position = new Vector3(referenceDoor.transform.position.x + doorOffSetx, referenceDoor.transform.position.y + doorOffSety, referenceDoor.transform.position.z + doorOffSetz);
            uiFieldCode.gameObject.SetActive(false);
            doorStatus = true;
            doorTrigger.gameObject.SetActive(true);
           doorShader.SetColor("_Color", new Color(0.0f, 0.0f, 1.0f, 0.0f));


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
    

    // Update is called once per frame
   
}
