using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_weapon_melee_lightning_stick", menuName = "ScriptableObjects/Weapons/Melee/Lightning Stick")]
public class MeleeWeapon : ScriptableObject
{
    [SerializeField] private float _range;
    [SerializeField] private float _recoveryTime;
    [SerializeField] private int _damage;
    
    public float Range => _range; 
    public float RecoveryTime => _recoveryTime; 
    public int Damage => _damage;

    private struct Upgrade
    {
        public enum UpgradeType
        {
            Range,
            RecoveryTime,
            Damage
        }

        public UpgradeType Type;
        public float Amount;
    }
}
