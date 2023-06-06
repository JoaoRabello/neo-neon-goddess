using System.Collections;
using System.Collections.Generic;
using Animations;
using Inputs;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Combat
{
    public class AttackSystem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private AimSystem _aimSystem;
        [SerializeField] private CharacterAnimator _animator;
        [SerializeField] private Transform _shotsOrigin;
        
        [Header("Weapon Data")]
        [SerializeField] private MeleeWeapon _weapon;
        
        [Header("Hit Scan properties")]
        [SerializeField] private LayerMask _hittableLayerMask;
        [SerializeField] private float _shotHitBoxSize;
        [SerializeField] private float _maxRange;
        
        private void OnEnable()
        {
            PlayerInputReader.Instance.ShootPerformed += ShootPerformed;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.ShootPerformed -= ShootPerformed;
        }

        private void ShootPerformed()
        {
            if (!_aimSystem.IsAiming) return;
            
            Shoot();
            _animator.SetParameterValue("isAiming", true);
        }

        private void Shoot()
        {
            RaycastHit[] enemies = new RaycastHit[10];
            var hitCount = Physics.SphereCastNonAlloc(_shotsOrigin.position, _shotHitBoxSize, _aimSystem.CurrentAimingDirection.ToVector3(transform.forward), enemies, _maxRange, _hittableLayerMask);

            if (hitCount <= 0) return;
            
            var enemy = enemies[0].collider;
            enemy.GetComponent<DummyHackable>().TakeHackShot(_weapon.Damage);
            Debug.Log($"Hit: {enemy.name} with {_weapon.Damage} damage");
        }
    }
}
