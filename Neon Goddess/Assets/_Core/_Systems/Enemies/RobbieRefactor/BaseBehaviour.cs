using Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseBehaviour : MonoBehaviour
{
    [SerializeField] protected CharacterAnimator _animator;
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    [SerializeField] protected LayerMask _playerLayerMask;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _specialAttackRange;
    [SerializeField] protected float _forgetRange;
    [SerializeField] protected float _range;
    [SerializeField] protected float _speed;

    [Header("EnemyBehaviour")]
    [SerializeField] protected HuntBehaviour _huntBehaviour;
    [SerializeField] protected IdleBehaviour _idleBehaviour;

    [Header("Health System")]
    [SerializeField] protected RobotHealthSystem _healthSystem;

    [Header("Waypoints")]
    [SerializeField] protected Transform _waypoint1;
    [SerializeField] protected Transform _waypoint2;

    [Header("SFX")]
    [SerializeField] protected AK.Wwise.Event _stabSoundEvent;
    [SerializeField] protected AK.Wwise.Event _stabHitSoundEvent;

    [Header("EnemyType")]
    [SerializeField] public EnemyType enemyType;
    public enum EnemyType
    {
        Halfie,
        Robbie,
        Eldritch
    }
    protected EnemyType _enemyType => enemyType;

    protected int _waypointIndex = 1;

    protected float _currentSpeed;

    protected bool _foundPlayer;
    protected bool _canMove = true;
    protected Transform _player;
    protected Vector3 _basePosition;

    protected bool _attackCooldown;
    protected bool _startedAttackCooldown;
    protected bool _hunting;
    protected bool _chasing;

    protected int size;
    protected Collider[] playerColliders = new Collider[3];

    private void Start()
    {
        _currentSpeed = _speed;
        _basePosition = transform.position;

        _healthSystem.OnHackedSuccessfully += OnHacked;
    }

    private void OnDisable()
    {
        _healthSystem.OnHackedSuccessfully -= OnHacked;
    }
    public void StartIdle()
    {
        _idleBehaviour.StartIdle();
    }

    public void StartHunt()
    {
        _hunting = true;
        _chasing = true;
    }
    private void OnHacked()
    {
        Stop();
        _hunting = false;
        _chasing = false;
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

}
