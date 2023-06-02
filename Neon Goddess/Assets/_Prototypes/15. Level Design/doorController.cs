using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class doorController : MonoBehaviour
{
    public int doorMovementDistance;
    public GameObject door;
    public GameObject astrid;
    public bool collisionStatus = false;
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
         if (collisionStatus = true )
        {
            door.transform.position = new Vector3(door.transform.position.x - doorMovementDistance, door.transform.position.y , door.transform.position.z);
          
            


        }
        // codigo no fieldcode -> triedPassword
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collisionStatus = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collisionStatus = false;
            door.transform.position = new Vector3(door.transform.position.x + doorMovementDistance, door.transform.position.y, door.transform.position.z);
        }
    }
}
