using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider _physicalHealthBar;
    [SerializeField] private Slider _mentalHealthBar;
    
    [Header("Data")]
    [SerializeField] private int _physicalMaxHealth;
    [SerializeField] private int _mentalMaxHealth;
    
    public enum HealthType
    {
        Physical,
        Mental
    }
    
    private int _currentPhysicalHealth;
    private int _currentMentalHealth;

    private void Start()
    {
        SetupHealths();
    }

    private void SetupHealths()
    {
        _currentPhysicalHealth = _physicalMaxHealth;
        _currentMentalHealth = _mentalMaxHealth;
        
        UpdatePhysicalBar();
        UpdateMentalBar();
    }
    
    public void TakePhysicalDamage(int amount)
    {
        _currentPhysicalHealth -= amount;

        UpdatePhysicalBar();
    }
    
    public void TakeMentalDamage(int amount)
    {
        _currentMentalHealth -= amount;

        UpdateMentalBar();
    }

    public void HealPhysical(int amount)
    {
        _currentPhysicalHealth += amount;
        _currentPhysicalHealth = Mathf.Clamp(_currentPhysicalHealth, 1, _physicalMaxHealth);
        
        UpdatePhysicalBar();
    }

    public void HealMental(int amount)
    {
        _currentMentalHealth += amount;
        _currentMentalHealth = Mathf.Clamp(_currentMentalHealth, 1, _mentalMaxHealth);
        
        UpdateMentalBar();
    }

    private void UpdatePhysicalBar()
    {
        _physicalHealthBar.value = (float) _currentPhysicalHealth / _physicalMaxHealth;
    }
    
    private void UpdateMentalBar()
    {
        _mentalHealthBar.value = (float) _currentMentalHealth / _mentalMaxHealth;

    }
}