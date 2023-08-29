using System;
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
    private bool isOpen = false;
    private int _interactionCount;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public bool doorMoving = false;
    public float doorMoveDistancex;
    public float doorMoveDistancey;
    public float doorMoveDistancez;
    public float speed = 10.0f;

    public Action<IInteractable> OnInteractUpdateIcon { get; set; }
    public Action<IInteractable> OnStateChangeUpdateIcon { get; set; }
    
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
        if (!doorMoving)
        {
            Open();
        }
        _interactionCount++;
    }

    public bool IsLocked()
    {
        return _locked;
    }

    public IInteractable.InteractableType GetInteractableType()
    {
        return _interactableType;
    }

    public bool Unlock()
    {
        if (_lockType == lockType.Item)
        {
            if (!PlayerInventoryObserver.Instance.TryConsumeItem(_keyItem)) return false;
            LockedDoor = false;
            
            OnStateChangeUpdateIcon?.Invoke(this);
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
        if (isOpen)
        {
            MoveDoor();
        }
    }
    void MoveDoor()
    {
        if (isOpen)
        {
            targetPosition = initialPosition;
        }
        else
        {
            AkSoundEngine.PostEvent("doorSlideOpen", gameObject);
            targetPosition = initialPosition + new Vector3(doorMoveDistancex, doorMoveDistancey, doorMoveDistancez);
        }
        isOpen = !isOpen;
        doorMoving = true;
    }

    public void shaderUnlocked()
    {
        if (doorShaderRenderer is not null)
        {
            doorShaderRenderer.materials[1].SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.0f));
            doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(0.0f, 4342.935f, 0.0f, 1.0f));
            doorShaderRenderer.materials[2].SetInt("_Status", 1);
        }
        if(doorLight is not null) doorLight.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        if(doorLight2 is not null) doorLight2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    }

    public void shaderLocked()
    {
        if (doorShaderRenderer is not null)
        {
            doorShaderRenderer.materials[1].SetColor("_Color", new Color(1.0f, 0.0f, 0.0f, 0.0f));
            doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(4342.935f, 0.0f, 0.0f, 1.0f));
            doorShaderRenderer.materials[2].SetInt("_Status", 0);
        }
        
        if(doorLight is not null) doorLight.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        if(doorLight2 is not null) doorLight2.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen == true)
        {
            AkSoundEngine.PostEvent("doorSlideClose", gameObject);
            MoveDoor();
        }
    }
}
