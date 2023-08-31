using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private Item _item;
    [SerializeField] private int _amount;
    [SerializeField] private GameObject dialogue;
    [SerializeField] private Dialogue _dialogue;
    private InventoryHolder _playerInventoryHolder;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInventoryHolder = other.GetComponentInParent<InventoryHolder>();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInventoryHolder = null;
        }
    }

    public Action<IInteractable> OnInteractUpdateIcon { get; set; }
    public Action<IInteractable> OnStateChangeUpdateIcon { get; set; }
    public void Interact()
    {
        if (_playerInventoryHolder is null) return;

        if (!_playerInventoryHolder.TryAddItem(_item, _amount)) return;
        
        ChatDialogueReader.Instance.PlayDialogue(_dialogue);
        
        Destroy(gameObject);
    }

    public IInteractable.InteractableType GetInteractableType()
    {
        return IInteractable.InteractableType.Common;
    }

    public bool HasInteractedOnce()
    {
        return false;
    }

    public bool IsLocked()
    {
        return false;
    }
}
