using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStation : MonoBehaviour
{
    [SerializeField] private HealthSystem.HealthType _healType;
    [SerializeField] private int _healAmount;
    
    public void OnTouchHealStation(Collider playerCollider)
    {
        var playerHealthSystem = playerCollider.GetComponentInParent<HealthSystem>();

        switch (_healType)
        {
            case HealthSystem.HealthType.Physical:
                playerHealthSystem.HealPhysical(_healAmount);
                break;
            case HealthSystem.HealthType.Mental:
                playerHealthSystem.HealMental(_healAmount);
                break;
        }
    }
}
