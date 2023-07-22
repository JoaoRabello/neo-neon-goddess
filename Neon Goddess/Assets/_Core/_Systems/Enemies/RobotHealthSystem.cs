using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealthSystem : MonoBehaviour, IHackable
{
    [SerializeField] private int _systemResistance;
    [SerializeField] private int _systemArmor;

    public Action OnHackedSuccessfully;
    
    private int _currentSystemResistance;
    
    private bool _isBeingHacked;
    private bool _isHacked;

    public bool IsBeingHacked => _isBeingHacked;
    public bool IsHacked => _isHacked;

    private void Start()
    {
        _currentSystemResistance = _systemResistance;
    }

    public void TakeHackShot(int damageAmount)
    {
        Debug.Log($"[RobotHealthSystem] TakeHackShot | Hit: {gameObject.name} with {damageAmount} damage");

        if (_isHacked) return;
        
        var damageTaken = damageAmount - _systemArmor;

        if (damageTaken >= _currentSystemResistance)
        {
            _currentSystemResistance = 0;
            
            Hack();
        }
        else
        {
            _currentSystemResistance -= damageTaken;
        }
    }

    public void Hack()
    {
        if (_isHacked) return;
        
        _isHacked = true;
        
        OnHackedSuccessfully?.Invoke();
        
        Debug.Log($"[RobotHealthSystem] TakeHackShot | Hacked successfully");
    }

    public void StartHack(float timeToHack)
    {
        _isBeingHacked = true;
    }

    public void CancelHack()
    {
        _isBeingHacked = false;
    }
}
