using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaEventTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider> _onTriggerEnterEvent;
    [SerializeField] private UnityEvent<Collider> _onTriggerExitEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onTriggerEnterEvent?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onTriggerExitEvent?.Invoke(other);
        }
    }
}
