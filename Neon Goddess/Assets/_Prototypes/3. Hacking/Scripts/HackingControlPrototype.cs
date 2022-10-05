using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingControlPrototype : MonoBehaviour
{
    [SerializeField] private AimingSystemPrototype _aimingSystem;
    [SerializeField] private HackAmmoData _hackAmmoData;

    [SerializeField] private HackAmmoData.HackAmmoType _currentAmmoType;

    private void OnEnable()
    {
        _aimingSystem.OnShot += TryShootTarget;
    }

    private void OnDisable()
    {
        _aimingSystem.OnShot -= TryShootTarget;
    }

    private void TryShootTarget(GameObject target)
    {
        var hackable = target.gameObject.GetComponent<IHackable>();
        if(hackable is null) return;
        
        ShootHackAmmo(hackable);
    }

    private void ShootHackAmmo(IHackable target)
    {
        target.TakeHackShot(_hackAmmoData.GetAmmoDamageByType(_currentAmmoType));
    }
}