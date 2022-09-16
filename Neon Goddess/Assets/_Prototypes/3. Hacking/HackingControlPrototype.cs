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
        var hackable = target.gameObject.GetComponent<IHackable>();
        if(hackable is null) return;
        
        if (_instantHack) Hack(hackable);
    }

    private void Hack(IHackable target)
    {
        if(_instantHack)
            target.Hack();
    }
}