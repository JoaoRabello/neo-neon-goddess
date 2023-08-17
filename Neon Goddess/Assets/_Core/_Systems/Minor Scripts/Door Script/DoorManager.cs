using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;

public class DoorManager : MonoBehaviour, IInteractable
{
    public enum lockType
    {
        Password,
        Item,
        Event,
        Naotem
    }

    [Header("Interaction")]
    [SerializeField] private IInteractable.InteractableType _interactableType;

    [Header("Door Information")]
    [SerializeField] bool isLockable;
    [SerializeField] GameObject doorObject;

    [Header("Lock Information")]
    [SerializeField] bool LockedDoor;
    [SerializeField] lockType _lockType;
    [SerializeField] KeyItem _keyItem;
    public Renderer doorShaderRenderer;
    public float doorEmissionIntensity;
    public Light doorLight;
    public Light doorLight2;

    private bool _locked => LockedDoor;
    private bool _open = false;
    private int _interactionCount;
    private bool isPlayerInsideTrigger;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public bool doorMoving = false;
    public float doorMoveDistancex;
    public float doorMoveDistancey;
    public float doorMoveDistancez;
    public float speed = 10.0f;

    void Start()
    {
        initialPosition = doorObject.transform.position;
    }

    void Update()
    {
        if (doorMoving)
        {
            doorObject.transform.position = Vector3.Lerp(doorObject.transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(doorObject.transform.position, targetPosition) < 0.01f)
            {
                doorMoving = false;
            }
        }
        if (isLockable)
        {
            switch (_locked)
            {
                case true:
                    shaderLocked();
                    break;
                case false:
                    shaderUnlocked();
                    break;
            }
        }
    }

    public bool HasInteractedOnce()
    {
        return _interactionCount > 0;
    }

    public void Interact()
    {
        if (isPlayerInsideTrigger && !doorMoving)
        {
            AkSoundEngine.PostEvent("doorSlideOpen", gameObject);
            Open();
        }
        _interactionCount++;
    }

    public bool IsLocked()
    {
        return _locked;
    }

    IInteractable.InteractableType IInteractable.GetType()
    {
        return _interactableType;
    }

    public bool Unlock()
    {
        if (_lockType == lockType.Item)
        {
            if (!PlayerInventoryObserver.Instance.TryConsumeItem(_keyItem)) return false;
            LockedDoor = false;
            return true;
        }
        return false; 
    }
    public void Lock()
    {

    }
    
    public void Open()
    {
        if (!isLockable)
        {
            MoveDoor();
        }
        else
        {
            if (_locked)
            {
                if (Unlock())
                {
                    MoveDoor();
                }
            }
            else
            {
                MoveDoor();
            }

        }
    }

    public void Close()
    {
        if(_open) 
        {
            MoveDoor();   
        }
    }
    void MoveDoor()
    {
        Debug.Log("MoveDoor()");
        if (_open)
        {
            targetPosition = initialPosition;
        }
        else
        {
            targetPosition = initialPosition + new Vector3(doorMoveDistancex, doorMoveDistancey, doorMoveDistancez);
        }
        _open = !_open;
        doorMoving = true;
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
        if (other.CompareTag("Player") && _open == true)
        {
            isPlayerInsideTrigger = false;
            AkSoundEngine.PostEvent("doorSlideClose", gameObject);
            MoveDoor();
        }
        else if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
        }
    }

    public void shaderUnlocked()
    {
        doorShaderRenderer.materials[1].SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.0f));
        doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(0.0f, 4342.935f, 0.0f, 1.0f));
        doorLight.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        doorLight2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        doorShaderRenderer.materials[2].SetInt("_Status", 1);
    }

    public void shaderLocked()
    {
        doorShaderRenderer.materials[1].SetColor("_Color", new Color(1.0f, 0.0f, 0.0f, 0.0f));
        doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(4342.935f, 0.0f, 0.0f, 1.0f));
        doorLight.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        doorLight2.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        doorShaderRenderer.materials[2].SetInt("_Status", 0);
    }
}
