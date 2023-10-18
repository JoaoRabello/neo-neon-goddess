using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableDoorLock : MonoBehaviour, IHackable
{
    [SerializeField] private DoorManager _door;
    [SerializeField] private Transform _hackPosition;
    
    private bool _isHacked;
    public bool IsHacked => _isHacked;
    
    public void SetAsPossibleTarget(bool value)
    {
    }

    public void SetAsCurrentTarget(bool value)
    {
    }

    public void TakeHackShot(int damageAmount)
    {
        if(_isHacked) return;
        
        Hack();
    }

    public void Hack()
    {
        _isHacked = true;
        _door.Unlock();
    }

    public void StartHack(float timeToHack)
    {
    }

    public void CancelHack()
    {
    }

    public Vector3 GetHeadPosition()
    {
        return _hackPosition.position;
    }

    public Vector3 GetTorsoPosition()
    {
        return _hackPosition.position;
    }

    public Vector3 GetLegsPosition()
    {
        return _hackPosition.position;
    }
}
