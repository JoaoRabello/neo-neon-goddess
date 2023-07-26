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

        private AimDirection _currentAimingDirection;
        private Vector3 _currentAimingDirectionVector3;
        private bool _isAiming;
        public LightningStickerVFXManeger lightningVFX;
        public bool IsAiming => _isAiming;
        public bool IsUsingAutoAim => _useAutoAim;
        public bool HasTarget => _automaticAimCurrentTarget != null;
        public bool weaponEquipped => _attackSystem.WeaponEquipped;
        public AimDirection CurrentAimingDirection => _currentAimingDirection;

        private void OnEnable()
        {
            PlayerInputReader.Instance.AimPerformed += AimPerformed;
            PlayerInputReader.Instance.AimCanceled += AimCanceled;
            PlayerInputReader.Instance.MovementPerformed += MovePerformed;
            PlayerInputReader.Instance.MovementCanceled += MoveCanceled;
        }

        private void OnDisable()
        {
            PlayerInputReader.Instance.AimPerformed -= AimPerformed;
            PlayerInputReader.Instance.AimCanceled -= AimCanceled;
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
            
            var results = new Collider[10];
            var enemyCount = Physics.OverlapSphereNonAlloc(transform.position, _aimRange, results, _hittableLayerMask);

            if (enemyCount <= 0)
            {
                _automaticAimCurrentTarget = null;
                return;
            }

            var distance = 99999f;
            var enemyIndex = 0;

            for (int i = 0; i < enemyCount; i++)
            {
                var distanceToCurrentEnemy = Vector3.Distance(transform.position, results[i].transform.position);
                if (distanceToCurrentEnemy >= distance) continue;

                distance = distanceToCurrentEnemy;
                enemyIndex = i;
            }
            
            _automaticAimCurrentTarget = results[enemyIndex].transform;
            
            Debug.Log($"[AimSystem] Update | Found {enemyCount} enemies. Current target is {_automaticAimCurrentTarget.name}");
            
            var thisTransform = transform;
            thisTransform.forward = (_automaticAimCurrentTarget.position - thisTransform.position).normalized;
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
