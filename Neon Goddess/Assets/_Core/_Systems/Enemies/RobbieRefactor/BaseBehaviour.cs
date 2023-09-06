using Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseBehaviour : MonoBehaviour
{
    [SerializeField] public CharacterAnimator _animator;
    [SerializeField] public NavMeshAgent _navMeshAgent;
    [SerializeField] public LayerMask _playerLayerMask;
    [SerializeField] public float _attackRange;
    [SerializeField] public float _specialAttackRange;
    [SerializeField] public float _forgetRange;
    [SerializeField] public float _range;
    [SerializeField] public float _speed;

    [Header("EnemyBehaviour")]
    [SerializeField] public HuntBehaviour _huntBehaviour;
    [SerializeField] public IdleBehaviour _idleBehaviour;

    [Header("Health System")]
    [SerializeField] public RobotHealthSystem _healthSystem;

    [Header("Waypoints")]
    [SerializeField] public Transform _waypoint1;
    [SerializeField] public Transform _waypoint2;

    [Header("SFX")]
    [SerializeField] public AK.Wwise.Event _stabSoundEvent;
    [SerializeField] public AK.Wwise.Event _stabHitSoundEvent;

    [Header("EnemyType")]
    [SerializeField] public EnemyType enemyType;
    public enum EnemyType
    {
        Halfie,
        Robbie,
        Eldritch
    }
    public EnemyType _enemyType => enemyType;

    public int _waypointIndex = 1;

    public float _currentSpeed;

    public bool _foundPlayer;
    public bool _canMove = true;
    public Transform _player;
    public Vector3 _basePosition;

    public bool _attackCooldown;
    public bool _startedAttackCooldown;

    public bool _hunting;
    public bool _chasing;
    public bool _haunting;
    public bool _idle;
    public bool _patrol;
    public bool _lamentation;

    public int size;
    public Collider[] playerColliders = new Collider[3];

    private void Start()
    {
        _currentSpeed = _speed;
        _basePosition = transform.position;

        _healthSystem.OnHackedSuccessfully += OnHacked;

        Idle();
    }

    private void OnDisable()
    {
        _healthSystem.OnHackedSuccessfully -= OnHacked;
    }
    public void Idle()
    {
        _huntBehaviour.EndHunt();
        _idleBehaviour.StartIdle();
    }

    public void Hunt()
    {
        _idleBehaviour.EndIdle();
        _huntBehaviour.StartHunt();
    }
    private void OnHacked()
    {
        Stop();
        _huntBehaviour.EndHunt();
        _idleBehaviour.EndIdle();
        //_keyDropItem.transform.position = transform.position + transform.forward;
        //_keyDropItem.SetActive(true);
    }
    private void Stop()
    {
        _navMeshAgent.SetDestination(transform.position);
        _animator.SetParameterValue("isWalking", false);
        _animator.SetParameterValue("isAttacking", false);
        _animator.SetParameterValue("isSpecialAttacking", false);
        _animator.PlayAnimation("Idle", 0);
    }

    public void RotateTowards(Vector3 targetPos)
    {
        _navMeshAgent.updateRotation = true;
    }

    public void Move(Vector3 desiredPosition)
    {
        if (!_canMove) return;

        _navMeshAgent.SetDestination(desiredPosition);

        _animator.SetParameterValue("isWalking", true);
    }

}
