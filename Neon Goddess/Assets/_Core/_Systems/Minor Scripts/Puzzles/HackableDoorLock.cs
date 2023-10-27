using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HackableDoorLock : MonoBehaviour, IHackable
{
    [SerializeField] private DoorManager _door;
    [SerializeField] private Transform _hackPosition;

    [SerializeField] private UnityEvent _onHackEvent;
    
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
        
        _onHackEvent?.Invoke();
        gameObject.layer = 0;
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
