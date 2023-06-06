using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Tooltip("Camera to turn on")]
    [SerializeField] private Camera _camera;
    //public bool insidetrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.Instance.SelectCamera(_camera);
            //insidetrigger = true;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
             //insidetrigger = false;
        }
    }*/
}
