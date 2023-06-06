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

        [Tooltip("Our custom animator")] [SerializeField]
        private CharacterAnimator _animator;

        private AimDirection _currentAimingDirection;
        private Vector3 _currentAimingDirectionVector3;
        private bool _isAiming;

        public bool IsAiming => _isAiming;
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
            _isAiming = true;
            PlayerStateObserver.Instance.OnAimStart();

            _animator.SetParameterValue("isAiming", true);
        }

        private void AimCanceled()
        {
            _isAiming = false;
            PlayerStateObserver.Instance.OnAimEnd();

            _animator.SetParameterValue("isAiming", false);
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
