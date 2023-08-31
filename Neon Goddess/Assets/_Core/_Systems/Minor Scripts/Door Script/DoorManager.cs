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

    [Header("Door Dialogues")]
    [SerializeField] private Dialogue _withoutKeyDialogue;
    [SerializeField] private Dialogue _withKeyDialogue;
    
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

    [Header("SFX")] 
    [SerializeField] private AK.Wwise.Event _lockedDoorSoundEvent;
    [SerializeField] private AK.Wwise.Event _unlockDoorSoundEvent;

    public Action<IInteractable> OnInteractUpdateIcon { get; set; }
    public Action<IInteractable> OnStateChangeUpdateIcon { get; set; }
    
    void Start()
    {
        initialPosition = doorObject.transform.position;

        if (isLockable && _locked) UpdateShaderToLocked();
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
                    UpdateShaderToLocked();
                    break;
                case false:
                    UpdateShaderToUnlocked();
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
            
            ChatDialogueReader.Instance.PlayDialogue(_withKeyDialogue);

            UpdateShaderToUnlocked();
            
            OnStateChangeUpdateIcon?.Invoke(this);
            _unlockDoorSoundEvent.Post(gameObject);
            return true;
        }
        return false;
    }
    
    public void Lock()
    {
        UpdateShaderToLocked();
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
                    return;
                }

                ChatDialogueReader.Instance.PlayDialogue(_withoutKeyDialogue);
                _lockedDoorSoundEvent.Post(gameObject);
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

    public void UpdateShaderToUnlocked()
    {
        if (doorShaderRenderer != null)
        {
            if (doorShaderRenderer.materials.Length > 0)
            {
                doorShaderRenderer.materials[1].SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.0f));
                doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(0.0f, 4342.935f, 0.0f, 1.0f));
                doorShaderRenderer.materials[2].SetInt("_Status", 1);
            }
        }
        if(doorLight != null) doorLight.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        if(doorLight2 != null) doorLight2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    }

    public void UpdateShaderToLocked()
    {
        if (doorShaderRenderer != null)
        {
            if (doorShaderRenderer.materials.Length > 0)
            {
                doorShaderRenderer.materials[1].SetColor("_Color", new Color(1.0f, 0.0f, 0.0f, 0.0f));
                doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(4342.935f, 0.0f, 0.0f, 1.0f));
                doorShaderRenderer.materials[2].SetInt("_Status", 0);
            }
        }
        
        if(doorLight != null) doorLight.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        if(doorLight2 != null) doorLight2.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
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
