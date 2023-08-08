using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animations;
using Inputs;
using Player;
using UnityEngine;

namespace Combat
{
    public class AimSystem : MonoBehaviour
    {
        public enum AimDirection
        {
            Up,
            Front,
            Down
        }
        
        [SerializeField] private AttackSystem _attackSystem;
        [SerializeField] private GameObject _weaponGameObject;
        [SerializeField] private GameObject _meleeWeaponGameObject;

        [Header("Automatic Aim")]
        [SerializeField] private bool _useAutoAim;
        [SerializeField] private LayerMask _hittableLayerMask;
        [SerializeField] private float _aimRange;
        
        private List<IHackable> _foundTargetHackables;
        private Transform _automaticAimCurrentTarget;
        private Transform _automaticAimFirstTarget;

        [Tooltip("Our custom animator")] 
        [SerializeField] private CharacterAnimator _animator;

        public LightningStickerVFXManeger lightningVFX;
        
        private AimDirection _currentAimingDirection;
        private Vector3 _currentAimingDirectionVector3;

        
        private bool _isAiming;

        private struct TargetAngle
        {
            public Transform Target;
            public float Angle;
        }
        
        private Collider[] _targets;
        private int _currentTargetCount;
        
        public bool IsAiming => _isAiming;
        public bool IsUsingAutoAim => _useAutoAim;
        public bool HasTarget => _automaticAimCurrentTarget != null;
        public bool weaponEquipped => _attackSystem.WeaponEquipped;
        public AimDirection CurrentAimingDirection => _currentAimingDirection;

        private void Start()
        {
            _foundTargetHackables = new List<IHackable>();
        }

        private void OnEnable()
        {
            PlayerInputReader.Instance.AimPerformed += AimPerformed;
            PlayerInputReader.Instance.AimPerformed += AimPerformed;
            PlayerInputReader.Instance.AimCanceled += AimCanceled;
            PlayerInputReader.Instance.MovementStarted += MoveStarted;
            PlayerInputReader.Instance.MovementPerformed += MovePerformed;
            PlayerInputReader.Instance.MovementCanceled += MoveCanceled;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.AimPerformed -= AimPerformed;
            PlayerInputReader.Instance.AimCanceled -= AimCanceled;
            PlayerInputReader.Instance.MovementStarted -= MoveStarted;
            PlayerInputReader.Instance.MovementPerformed -= MovePerformed;
            PlayerInputReader.Instance.MovementCanceled -= MoveCanceled;
        }

        private void AimPerformed()
        {
            if(PlayerStateObserver.Instance.CurrentState != PlayerStateObserver.PlayerState.Free) return;
            
            StopAllCoroutines();
            StartCoroutine(StartAiming());
            PlayerStateObserver.Instance.OnAimStart();

            if (_attackSystem.WeaponEquipped)
            {
                _animator.SetParameterValue("isAiming", true);
                _weaponGameObject.SetActive(true);
            }
            else
            {
                _animator.SetParameterValue("isAimingMelee", true);
                _meleeWeaponGameObject.SetActive(true);
            }

            UpdateTargets();
            _automaticAimFirstTarget = _automaticAimCurrentTarget;
        }

        private void UpdateTargets()
        {
            _targets = new Collider[10];

            if (!TryGetTargets(out _targets, out _currentTargetCount)) return;

            //TODO: Talvez remover esse getcomponent por falta de necessidade de usar o ihackable
            for (int i = 0; i < _currentTargetCount; i++)
            {
                var hackable = _targets[i].GetComponent<IHackable>();

                if (_foundTargetHackables.Contains(hackable)) continue;

                AimCrossHairManager.Instance.RenderCrossHair(hackable, true, false);
                _foundTargetHackables.Add(hackable);
            }

            var enemyIndex = GetClosestTargetIndexInColliderArray(_currentTargetCount, _targets);

            if (_automaticAimCurrentTarget == _targets[enemyIndex].transform) return;
            if (_automaticAimFirstTarget == _targets[enemyIndex].transform) return;
            
            _automaticAimCurrentTarget = _targets[enemyIndex].transform;
            _automaticAimFirstTarget = _targets[enemyIndex].transform;
                
            AimCrossHairManager.Instance.RenderCrossHair(_automaticAimCurrentTarget.GetComponent<IHackable>(), true, true);
        }

        private int GetClosestTargetIndexInColliderArray(int enemyCount, Collider[] results)
        {
            var distance = 99999f;
            var enemyIndex = 0;

            for (int i = 0; i < enemyCount; i++)
            {
                var distanceToCurrentEnemy = Vector3.Distance(transform.position, results[i].transform.position);
                if (distanceToCurrentEnemy >= distance) continue;

                distance = distanceToCurrentEnemy;
                enemyIndex = i;
            }

            return enemyIndex;
        }

        private Transform GetNextTargetWithLessAngleDistance(Collider[] results, float xInput)
        {
            var negativeAngleList = new List<TargetAngle>();
            var positiveAngleList = new List<TargetAngle>();
            
            for (var index = 0; index < _currentTargetCount; index++)
            {
                var targetTransform = results[index].transform;
                
                if(targetTransform == _automaticAimCurrentTarget) continue;
                
                var currentTargetDirection = (_automaticAimCurrentTarget.position - transform.position).normalized;
                var targetDirection = (targetTransform.position - transform.position).normalized;
                var signedAngle = Vector3.SignedAngle(currentTargetDirection, targetDirection, transform.up);

                var targetAngle = new TargetAngle
                {
                    Target = targetTransform,
                    Angle = signedAngle
                };
                
                switch (signedAngle)
                {
                    case > 0.01f:
                        positiveAngleList.Add(targetAngle);
                        break;
                    case < -0.01f:
                        negativeAngleList.Add(targetAngle);
                        break;
                }
            }
            
            positiveAngleList = positiveAngleList.OrderBy(targetAngle => targetAngle.Angle).ToList();
            negativeAngleList = negativeAngleList.OrderByDescending(targetAngle => targetAngle.Angle).ToList();

            return xInput switch
            {
                > 0.1f => positiveAngleList[0].Target,
                < -0.1f => negativeAngleList[0].Target,
                _ => _automaticAimCurrentTarget
            };
        }

        private bool TryGetTargets(out Collider[] results, out int enemyCount)
        {
            results = new Collider[10];
            enemyCount = Physics.OverlapSphereNonAlloc(transform.position, _aimRange, results, _hittableLayerMask);

            if (enemyCount <= 0)
            {
                _automaticAimCurrentTarget = null;
                _automaticAimFirstTarget = null;
                return false;
            }

            return true;
        }

        private IEnumerator StartAiming()
        {
            yield return new WaitForSeconds(0.5f);
            
            _isAiming = true;
            lightningVFX.EffectActivator();
        }

        private void AimCanceled()
        {
            _isAiming = false;
            
            StopAllCoroutines();
            StartCoroutine(StopAiming());
            PlayerStateObserver.Instance.OnAimEnd();

            _animator.SetParameterValue("isAiming", false);
            _animator.SetParameterValue("isAimingMelee", false);
        
            lightningVFX.EffectActivator();

            var foundHackablesCopy =  _foundTargetHackables.ToList();
            
            AimCrossHairManager.Instance.CancelAim();
            foreach (var hackable in foundHackablesCopy)
            {
                _foundTargetHackables.Remove(hackable);
            }

            _automaticAimCurrentTarget = null;
            _automaticAimFirstTarget = null;
        }
        
        private IEnumerator StopAiming()
        {
            yield return new WaitForSeconds(0);
            
            _weaponGameObject.SetActive(false);
            _meleeWeaponGameObject.SetActive(false);
        }

        private void MoveStarted(Vector2 movementInput)
        {
            if (Mathf.Abs(movementInput.x) < 0.1f) return;
            
            SwitchTarget(movementInput.x);
        }
        
        private void MovePerformed(Vector2 movementInput)
        {
            _currentAimingDirection = movementInput.y switch
            {
                >= 0.1f => AimDirection.Up,
                <= -0.1f => AimDirection.Down,
                _ => AimDirection.Front
            };
        }

        private void MoveCanceled()
        {
            _currentAimingDirection = AimDirection.Front;
        }

        private void SwitchTarget(float xInput)
        {
            if(_currentTargetCount <= 0) return;
            if(_automaticAimCurrentTarget == null) return;

            var automaticAimCurrentTargetHackable = _automaticAimCurrentTarget.GetComponent<IHackable>();
            
            AimCrossHairManager.Instance.RenderCrossHair(automaticAimCurrentTargetHackable, false, true);
            _automaticAimCurrentTarget = GetNextTargetWithLessAngleDistance(_targets, xInput);
            AimCrossHairManager.Instance.RenderCrossHair(automaticAimCurrentTargetHackable, true, true);
        }

        private void Update()
        {
            if (PlayerStateObserver.Instance.CurrentState == PlayerStateObserver.PlayerState.Dead)
            {
                AimCanceled();
            }
            if (!_isAiming) return;

            _currentAimingDirectionVector3 = _currentAimingDirection.ToVector3(transform.forward);
            
            switch (_currentAimingDirection)
            {
                case AimDirection.Up:
                    _animator.SetParameterValue("aimDirection", 0f);
                    break;
                case AimDirection.Front:
                    _animator.SetParameterValue("aimDirection", 0.5f);
                    break;
                case AimDirection.Down:
                    _animator.SetParameterValue("aimDirection", 1f);
                    break;
            }
            
            if(!_useAutoAim) return;

            UpdateTargets();

            if(_automaticAimCurrentTarget == null) return;

            var thisTransform = transform;
            var newForward = (_automaticAimCurrentTarget.position - thisTransform.position).normalized;
            thisTransform.forward = new Vector3(newForward.x, thisTransform.forward.y, newForward.z);
        }
    }
    
    public static class AimDirectionExtensions
    {
        public static Vector3 ToVector3(this AimSystem.AimDirection aimDirection, Vector3 forward)
        {
            return aimDirection switch
            {
                AimSystem.AimDirection.Up => forward + Vector3.up * 0.75f,
                AimSystem.AimDirection.Front => forward,
                AimSystem.AimDirection.Down => forward + Vector3.down * 0.75f,
                _ => forward
            };
        }
    }
}
