using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public enum InteractableType
    {
        Common,
        Dialogue,
        Door,
        ShutDoor
    }

    public Action<IInteractable> OnInteractUpdateIcon { get; set; }
    public Action<IInteractable> OnStateChangeUpdateIcon { get; set; }
    public void Interact();
    public InteractableType GetInteractableType();
    public bool HasInteractedOnce();
    public bool IsLocked();
}
