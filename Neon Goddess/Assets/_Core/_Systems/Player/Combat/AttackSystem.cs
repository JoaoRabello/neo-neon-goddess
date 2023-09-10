using System;
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
        
        [Header("SFX")]
        [SerializeField] private SFXPlayer _shootingSfxPlayer;
        [SerializeField] private AK.Wwise.Event _stickSwing;
        [SerializeField] private AK.Wwise.Event _gunHit;
        [SerializeField] private AK.Wwise.Event _stickHit;
        [SerializeField] private AK.Wwise.RTPC _stickHitRTPC;
        
        [Header("Weapon Data")]
        [SerializeField] public bool _weaponEquipped;
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
        public bool IsOnAttackAnimation => _isOnAnimation;
        
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
            if(PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Dead) return;
            Debug.Log("ShootPerformed");
            Shoot();
            
            if(_weaponEquipped)
                _animator.SetParameterValue("isAiming", true);
        }
        
        private void ChangeWeapon()
        {
            if(PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Aiming) return;
            if(_aimSystem.IsAiming) return;
            
            _weaponEquipped = !_weaponEquipped;
        }

        private void Shoot()
        {
            if(_isOnAnimation) return;
            
            _isOnAnimation = true;
            
            PlayerStateObserver.Instance.OnAimEnd();
            PlayerStateObserver.Instance.OnAnimationStart();

            StartCoroutine(PlayMuzzleEffect());

            if (_weaponEquipped)
            {
                _shootingSfxPlayer.PlaySFX();
            }
            else
            {
                _shootingSfxPlayer.PlaySFX(_stickSwing);
            }

            _animator.PlayAndOnAnimationEndCallback(_weaponEquipped ? "Shot" : "Stab", AnimationEnded);

            RaycastHit[] enemies = new RaycastHit[10];
            int hitCount;
            
            if (_aimSystem.CurrentHackableTarget is null)
            {
                hitCount = Physics.SphereCastNonAlloc(_shotsOrigin.position, _shotHitBoxRadiusSize, 
                    _aimSystem.CurrentAimingDirection.ToVector3(transform.forward), enemies, 
                    _weaponEquipped ? _maxShootingRange : _maxMeleeRange, _hittableLayerMask);
            }
            else
            {
                var aimDirection = transform.forward;
                switch (_aimSystem.CurrentAimingDirection)
                {
                    case AimSystem.AimDirection.Up:
                        aimDirection = (_aimSystem.CurrentHackableTarget.GetHeadPosition() - transform.position).normalized;
                        break;
                    case AimSystem.AimDirection.Front:
                        aimDirection = (_aimSystem.CurrentHackableTarget.GetTorsoPosition() - transform.position).normalized;
                        break;
                    case AimSystem.AimDirection.Down:
                        aimDirection = (_aimSystem.CurrentHackableTarget.GetLegsPosition() - transform.position).normalized;
                        break;
                }

                hitCount = Physics.SphereCastNonAlloc(_shotsOrigin.position, _shotHitBoxRadiusSize, 
                    aimDirection, enemies, _weaponEquipped ? _maxShootingRange : _maxMeleeRange, _hittableLayerMask);
            }
            
            if (hitCount <= 0) return;
            
            var enemy = enemies[0].collider;

            if (_weaponEquipped)
            {
                //TODO: Checar com a Zarina o motivo desse evento não estar sendo encontrado
                _shootingSfxPlayer.PlaySFX(_gunHit);
            }
            else
            {
                _shootingSfxPlayer.PlaySFX(_stickHit);
            }
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
