using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageSpikes : MonoBehaviour
{
    [SerializeField] private HealthSystem.HealthType _damageType;
    [SerializeField] private int _damageAmount;
    
    public void OnSpikeTouched(Collider playerCollider)
    {
        var playerHealthSystem = playerCollider.GetComponentInParent<HealthSystem>();
        
        switch (_damageType)
        {
            case HealthSystem.HealthType.Physical:
                playerHealthSystem.TakePhysicalDamage(_damageAmount);
                break;
            case HealthSystem.HealthType.Mental:
                playerHealthSystem.TakeMentalDamage(_damageAmount);
                break;
        }
    }
}
