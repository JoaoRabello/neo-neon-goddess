using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeCombat : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private MeleeWeapon _weapon;
    
    private InputActions _inputActions;
    
    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Prototype.Melee.performed += MeleePerformed;
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Prototype.Melee.performed -= MeleePerformed;
        _inputActions.Disable();
    }
    
    private void MeleePerformed(InputAction.CallbackContext context)
    {
        Collider[] enemies = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, _weapon.Range, enemies, _enemyLayerMask);

        for (int i = 0; i < size; i++)
        {
            var enemy = enemies[i];
            Debug.Log($"Hit: {enemy.name}");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _weapon.Range);
    }
}
