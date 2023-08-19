using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StopEventOnDestroy : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event _eventToStop;

    private void OnDestroy()
    {
        _eventToStop.Stop(gameObject);
    }
}
