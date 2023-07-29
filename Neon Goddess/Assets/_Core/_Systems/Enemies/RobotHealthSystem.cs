using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealthSystem : MonoBehaviour, IHackable
{
    [SerializeField] private int _systemResistance;
    [SerializeField] private int _systemArmor;
    
    [Header("UI")]
    [SerializeField] private GameObject _possibleTargetIcon;
    [SerializeField] private GameObject _currentTargetIcon;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem _hackedVFX;
    
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

    public void SetAsPossibleTarget(bool value)
    {
        _possibleTargetIcon.SetActive(value);
    }

    public void SetAsCurrentTarget(bool value)
    {
        _possibleTargetIcon.SetActive(!value);
        _currentTargetIcon.SetActive(value);
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
        
        _hackedVFX.gameObject.SetActive(true);
        _hackedVFX.Play();
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
