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
        [SerializeField] private bool _weaponEquipped;
        [SerializeField] private MeleeWeapon _weapon;
        
        [Header("Hit Scan properties")]
        [SerializeField] private LayerMask _hittableLayerMask;
        [SerializeField] private float _shotHitBoxRadiusSize;
        [SerializeField] private float _maxShootingRange;
        [SerializeField] private float _maxMeleeRange;
        
        [Header("Prototype")]
        [SerializeField] private GameObject _fakeMuzzleEffect;
        [SerializeField] private float _muzzleEffectTime;

        public bool WeaponEquipped => _weaponEquipped;
        
        private bool _isOnAnimation;
        
        private void OnEnable()
        {
            PlayerInputReader.Instance.ShootPerformed += ShootPerformed;
            PlayerInputReader.Instance.ChangeWeaponPerformed += ChangeWeapon;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.ShootPerformed -= ShootPerformed;
            PlayerInputReader.Instance.ChangeWeaponPerformed -= ChangeWeapon;
        }

        private void ShootPerformed()
        {
            if (!_aimSystem.IsAiming) return;
            
            Shoot();
            
            if(_weaponEquipped)
                _animator.SetParameterValue("isAiming", true);
        }
        
        private void ChangeWeapon()
        {
            _weaponEquipped = !_weaponEquipped;
        }

        private void Shoot()
        {
            if(_isOnAnimation) return;
            
            PlayerStateObserver.Instance.OnAimEnd();
            PlayerStateObserver.Instance.OnAnimationStart();

            StartCoroutine(PlayMuzzleEffect());
            
            _isOnAnimation = true;

            _animator.PlayAndOnAnimationEndCallback(_weaponEquipped ? "Shot" : "Stab", AnimationEnded);

            RaycastHit[] enemies = new RaycastHit[10];
            var hitCount = Physics.SphereCastNonAlloc(_shotsOrigin.position, _shotHitBoxRadiusSize, 
                _aimSystem.CurrentAimingDirection.ToVector3(transform.forward), enemies, 
                _weaponEquipped ? _maxShootingRange : _maxMeleeRange, _hittableLayerMask);

            if (hitCount <= 0) return;
            
            var enemy = enemies[0].collider;
            enemy.GetComponent<IHackable>().TakeHackShot(_weapon.Damage);
        }

        private IEnumerator PlayMuzzleEffect()
        {
            _fakeMuzzleEffect.SetActive(true);
            yield return new WaitForSeconds(_muzzleEffectTime);
            _fakeMuzzleEffect.SetActive(false);
        }

        private void AnimationEnded()
        {
            _isOnAnimation = false;
            PlayerStateObserver.Instance.OnAnimationEnd();
            
            if(_aimSystem.IsAiming)
                PlayerStateObserver.Instance.OnAimStart();
        }
    }
}
