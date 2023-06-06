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
    public string triedPassword;
    public GameObject referenceDoor;
    public int doorOffSet;
    public bool doorStatus = false;

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

        if (triedPassword != password)
        {
            uiFieldCode.gameObject.SetActive(true);
        }
                
        else if (triedPassword == password && doorStatus == false)
        {
            referenceDoor.transform.position = new Vector3(referenceDoor.transform.position.x, referenceDoor.transform.position.y + doorOffSet, referenceDoor.transform.position.z);
            uiFieldCode.gameObject.SetActive(false);
            doorStatus = true;


        }

        // codigo no fieldcode -> triedPassword
    }
    void Start()
    {
        
    }

    // Update is called once per frame
   
}
