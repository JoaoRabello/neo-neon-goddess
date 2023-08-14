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

    public void Interact();
    public InteractableType GetType();
}
