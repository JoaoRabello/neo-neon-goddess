using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class doorBehavor : MonoBehaviour
{
    public GameObject door;
    //public GameObject triggerObject;
    public float doorMoveDistance ;
    public bool isPlayerInsideTrigger = false;
    private InputActions _inputActions;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = door.transform.position;
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
        if (isPlayerInsideTrigger == true)
        {
            MoveDoor();
        }
    }
    void Update()
    {
        
    }

     void MoveDoor()
     {
         Vector3 targetPosition = initialPosition + new Vector3(doorMoveDistance, 0f, 0f);
         door.transform.position = targetPosition;
     }

  

    void ResetDoor()
    {
        door.transform.position = initialPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
            ResetDoor();
        }
    }
}
