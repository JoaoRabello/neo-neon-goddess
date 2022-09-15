using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingControlPrototype : MonoBehaviour
{
    [SerializeField] private AimingSystemPrototype _aimingSystem;
    [SerializeField] private bool _instantHack;

    private void OnEnable()
    {
        _aimingSystem.OnShot += StartHacking;
    }

    private void OnDisable()
    {
        _aimingSystem.OnShot -= StartHacking;
    }

    private void StartHacking(GameObject target)
    {
        // if(!target.gameObject.GetComponent<GameObject>()) return;
        
        if (_instantHack) Hack(target);
    }

    private void Hack(GameObject target)
    {
        if(target is null)
        {
            Debug.Log("Errou");
            return;
        }
        Debug.Log("Hacked " + target.name);
    }
}