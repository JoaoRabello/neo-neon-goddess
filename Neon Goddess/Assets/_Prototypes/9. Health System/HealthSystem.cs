using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Tooltip("Our custom CharacterAnimator")]
    [SerializeField] private CharacterAnimator _animator;
    
    [Header("UI")]
    [SerializeField] private Slider _physicalHealthBar;
    [SerializeField] private Slider _mentalHealthBar;
    [SerializeField] private InventoryRenderer _inventoryRenderer;
    
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
    
    public int CurrentPhysicalHealth => _currentPhysicalHealth;

    private void Start()
    {
        SetupHealths();
        _inventoryRenderer.HealItemUsed += HealByItem;
    }

    private void HealByItem(HealItem item)
    {
        switch (item.Type)
        {
            case HealItem.HealType.Physical:
                HealPhysical(item.Amount);
                break;
            case HealItem.HealType.Mental:
                HealMental(item.Amount);
                break;
            case HealItem.HealType.Both:
                HealPhysical(item.Amount);
                HealMental(item.Amount);
                break;
        }
    }

    private void OnDisable()
    {
        _inventoryRenderer.HealItemUsed -= HealByItem;
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
        
        _animator.PlayAnimation("Damage", 0);

        UpdatePhysicalBar();
    }
    
    public void TakePhysicalDamage(float percentage)
    {
        _currentPhysicalHealth -= Mathf.FloorToInt(_physicalMaxHealth * percentage);
        
        _animator.PlayAnimation("Damage", 0);

        UpdatePhysicalBar();
    }
    
    public void TakeDirectSetPhysicalDamage(int desiredHealthAmount)
    {
        _currentPhysicalHealth = desiredHealthAmount;

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
