using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int _physicalMaxHealth;
    [SerializeField] private int _mentalMaxHealth;
    
    private int _currentPhysicalHealth;
    private int _currentMentalHealth;

    private void SetupHealths()
    {
        _currentPhysicalHealth = _physicalMaxHealth;
        _currentMentalHealth = _mentalMaxHealth;
    }
    
    public void TakePhysicalDamage(int amount)
    {
        _currentPhysicalHealth -= amount;
    }
    
    public void TakeMentalDamage(int amount)
    {
        _currentMentalHealth -= amount;
    }

    public void HealPhysical(int amount)
    {
        _currentPhysicalHealth += amount;
        _currentPhysicalHealth = Mathf.Clamp(_currentPhysicalHealth, 1, _physicalMaxHealth);
    }

    public void HealMental(int amount)
    {
        _currentMentalHealth += amount;
        _currentMentalHealth = Mathf.Clamp(_currentMentalHealth, 1, _mentalMaxHealth);
    }
}
