using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class doorBehavor : MonoBehaviour
{
    public GameObject door;
    //public GameObject triggerObject;
    public float doorMoveDistancex ;
    public float doorMoveDistancey;
    public float doorMoveDistancez;
    public bool isPlayerInsideTrigger = false;
    private InputActions _inputActions;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    public bool doorIsMoving = false;
    public float speed = 10.0f;
    private bool doorIsOpen = false;
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
        Debug.Log("doorIsMoving: " + doorIsMoving);
        Debug.Log("isPlayerInsideTrigger: " + isPlayerInsideTrigger);
        Debug.Log("OnInteractPerformed called");
        if (isPlayerInsideTrigger == true && !doorIsMoving)
        {
            MoveDoor();
        }
    }
    void Update()
    {

        if (doorIsMoving)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(door.transform.position, targetPosition) < 0.01f)
            {
                doorIsMoving = false;
            }
        }
    }
     void MoveDoor()
     {
        Debug.Log("MoveDoor called");
        if (doorIsOpen)
        {
            targetPosition = initialPosition;
        }
       else
        {
            targetPosition = initialPosition + new Vector3(doorMoveDistancex, doorMoveDistancey, doorMoveDistancez);
        }
        doorIsOpen = !doorIsOpen;
        doorIsMoving = true;
    }

  

   /* void ResetDoor()
    {
        door.transform.position = initialPosition;
    }*/

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && doorIsOpen == true)
        {
            isPlayerInsideTrigger = false;
            //ResetDoor();
            MoveDoor();

        }
        else if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
        }
        

    }
}
