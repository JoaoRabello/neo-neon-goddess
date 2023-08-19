using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class DoorBehaviour : MonoBehaviour
{
    [SerializeField] private IInteractable.InteractableType _interactableType;
    
    public GameObject door;
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

    public void Interact()
    {
        if (isPlayerInsideTrigger && !doorIsMoving)
        {
            AkSoundEngine.PostEvent("doorSlideOpen", gameObject);
            MoveDoor();
        }
    }

    public IInteractable.InteractableType GetType()
    {
        return _interactableType;
    }

    public bool HasInteractedOnce()
    {
        return false;
    }

    public bool IsLocked()
    {
        return false;
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
            AkSoundEngine.PostEvent("doorSlideClose", gameObject);
            MoveDoor();
        }
        else if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
        }
    }
}
