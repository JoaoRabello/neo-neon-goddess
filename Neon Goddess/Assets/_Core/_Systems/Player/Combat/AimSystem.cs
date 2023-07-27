using System.Collections;
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
        [SerializeField] private Transform _automaticAimCurrentTarget;

        [Tooltip("Our custom animator")] 
        [SerializeField] private CharacterAnimator _animator;

        public LightningStickerVFXManeger lightningVFX;
        private AimDirection _currentAimingDirection;
        private Vector3 _currentAimingDirectionVector3;
        
        private bool _isAiming;

        private Collider[] _targets;
        private int _currentTargetCount;
        
        public bool IsAiming => _isAiming;
        public bool IsUsingAutoAim => _useAutoAim;
        public bool HasTarget => _automaticAimCurrentTarget != null;
        public bool weaponEquipped => _attackSystem.WeaponEquipped;
        public AimDirection CurrentAimingDirection => _currentAimingDirection;

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

            _targets = new Collider[10];

            if (!TryGetTargets(out _targets, out _currentTargetCount)) return;
            
            var enemyIndex = GetClosestTargetIndexInColliderArray(_currentTargetCount, _targets);

            _automaticAimCurrentTarget = _targets[enemyIndex].transform;
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
            var smallestAngle = 99999f;

            var closestTargets = new Transform[2];

            for (var index = 0; index < _currentTargetCount; index++)
            {
                var targetTransform = results[index].transform;
                
                if(targetTransform == _automaticAimCurrentTarget) continue;
                
                var currentTargetDirection = (_automaticAimCurrentTarget.position - transform.position).normalized;
                var targetDirection = (targetTransform.position - transform.position).normalized;
                var angleToTarget = Mathf.Acos(Vector3.Dot(currentTargetDirection, targetDirection)) * (180 / Mathf.PI);

                if (angleToTarget >= smallestAngle) continue;

                smallestAngle = angleToTarget;

                closestTargets[1] = closestTargets[0];
                closestTargets[0] = targetTransform;
            }

            if (closestTargets[1] == null)
            {
                if (closestTargets[0] == null)
                {
                    return _automaticAimCurrentTarget;
                }
                
                if (closestTargets[0].position.x > _automaticAimCurrentTarget.position.x)
                {
                    if (xInput > 0.1f)
                    {
                        Debug.Log($"[AimSystem] GetNextTarget... | Closest Target 0 is to the right and wanna right. Return closest");
                        return closestTargets[0];
                    }
                    if (xInput < 0.1f)
                    {
                        Debug.Log($"[AimSystem] GetNextTarget... | Closest Target 0 is to the right and wanna left. Return current");
                        return _automaticAimCurrentTarget;
                    }
                    
                    Debug.Log($"[AimSystem] GetNextTarget... | Closest Target 0 is to the right and Input is 0. Return current");
                    return  _automaticAimCurrentTarget;
                }
                if (closestTargets[0].position.x < _automaticAimCurrentTarget.position.x)
                {
                    if (xInput > 0.1f)
                    {
                        Debug.Log($"[AimSystem] GetNextTarget... | Closest Target 0 is to the left and wanna right. Return current");
                        return _automaticAimCurrentTarget;
                    }
                    if (xInput < 0.1f)
                    {
                        Debug.Log($"[AimSystem] GetNextTarget... | Closest Target 0 is to the left and wanna left. Return closest");
                        return closestTargets[0];
                    }
                    
                    return  _automaticAimCurrentTarget;
                }
            }
            
            if (xInput > 0.1f)
            {
                if (closestTargets[0].position.x > _automaticAimCurrentTarget.position.x)
                {
                    Debug.Log($"[AimSystem] GetNextTarget... | Return closestTarget 0 with x > currentTarget x");
                    return closestTargets[0];
                }
                
                if (closestTargets[1].position.x > _automaticAimCurrentTarget.position.x)
                {
                    Debug.Log($"[AimSystem] GetNextTarget... | Return closestTarget 1 with x > currentTarget x");
                    return closestTargets[1];
                }
            }
            
            if (xInput < -0.1f)
            {
                if (closestTargets[0].position.x < _automaticAimCurrentTarget.position.x)
                {
                    Debug.Log($"[AimSystem] GetNextTarget... | Return closestTarget 0 with x < currentTarget x");
                    return closestTargets[0];
                }
                if (closestTargets[1].position.x < _automaticAimCurrentTarget.position.x)
                {
                    Debug.Log($"[AimSystem] GetNextTarget... | Return closestTarget 1 with x < currentTarget x");
                    return closestTargets[1];
                }
            }
            
            Debug.Log($"[AimSystem] GetNextTarget... | Return closestTarget 0");
            return closestTargets[0];
        }

        private bool TryGetTargets(out Collider[] results, out int enemyCount)
        {
            results = new Collider[10];
            enemyCount = Physics.OverlapSphereNonAlloc(transform.position, _aimRange, results, _hittableLayerMask);

            if (enemyCount <= 0)
            {
                _automaticAimCurrentTarget = null;
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

            if (_attackSystem.WeaponEquipped)
            {
                _animator.SetParameterValue("isAiming", false);
            }
            else
            {
                _animator.SetParameterValue("isAimingMelee", false);
            }
            
            lightningVFX.EffectActivator();
        }
        
        private IEnumerator StopAiming()
        {
            yield return new WaitForSeconds(0.5f);
            
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
            //
            // var currentTargetIndex = 0;
            //
            // for (int i = 0; i < _currentTargetCount; i++)
            // {
            //     if(_targets[i].transform != _automaticAimCurrentTarget) continue;
            //
            //     currentTargetIndex = i;
            //     break;
            // }
            //
            // var factor = xInput > 0.1f ? +1 : -1;
            // var newIndex = Mathf.Clamp(currentTargetIndex + factor, 0, _currentTargetCount - 1);
            // _automaticAimCurrentTarget = _targets[newIndex].transform;
            
            _automaticAimCurrentTarget = GetNextTargetWithLessAngleDistance(_targets, xInput);
        }

        private void Update()
        {
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

            TryGetTargets(out _targets, out _currentTargetCount);
            
            if(_automaticAimCurrentTarget == null) return;

            var thisTransform = transform;
            var newForward = (_automaticAimCurrentTarget.position - thisTransform.position).normalized;
            thisTransform.forward = new Vector3(newForward.x, thisTransform.forward.y, newForward.z);
        }

        //TODO: Remove debug gizmos
        private void OnDrawGizmos()
        {
            if (!_isAiming) return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, _currentAimingDirectionVector3 * 3);
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
