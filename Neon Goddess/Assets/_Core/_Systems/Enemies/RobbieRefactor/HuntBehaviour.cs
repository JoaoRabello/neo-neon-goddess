using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.AI;
using Animations;
using static BaseBehaviour;
using FIMSpace.Basics;
using System;

public class HuntBehaviour : BaseBehaviour
{   
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _specialAttackRange;
    [SerializeField] private float _forgetRange;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;

    [Header("EnemyBehaviour")]
    [SerializeField] private BaseBehaviour _baseBehaviour;
    [SerializeField] private HauntBehaviour _hauntBehaviour;
    [SerializeField] private ChaseBehaviour _chaseBehaviour;

    [Header("Health System")]
    [SerializeField] private RobotHealthSystem _healthSystem;

    [Header("Waypoints")]
    [SerializeField] private Transform _waypoint1;
    [SerializeField] private Transform _waypoint2;

    [Header("SFX")]
    [SerializeField] private AK.Wwise.Event _stabSoundEvent;
    [SerializeField] private AK.Wwise.Event _stabHitSoundEvent;

    [Header("EnemyType")]
    [SerializeField] public EnemyType enemyType;
    public enum EnemyType
    {
        Halfie,
        Robbie,
        Eldritch
    }
    private EnemyType _enemyType => enemyType;

    private int _waypointIndex = 1;

    private float _currentSpeed;

    private bool _foundPlayer;
    private bool _canMove = true;
    private Transform _player;
    private Vector3 _basePosition;

    private bool _attackCooldown;
    private bool _startedAttackCooldown;

    private int size;
    Collider[] playerColliders = new Collider[3];

    private void Update()
    {
        if (_foundPlayer)
        {
            var distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distanceToPlayer <= _specialAttackRange)
            {
                _currentSpeed = 20;
                SpecialAttack();
            }
            else if (distanceToPlayer <= _attackRange)
            {
                Attack();
            }
            else
            {
                StartChase();
            }
        }
        else
        {
            if (_enemyType == EnemyType.Eldritch) 
            {
                StartHaunt();
            }
            else if(_enemyType == EnemyType.Robbie)
            {
                StartIdle();
            }

            if (size <= 0) return;

            _foundPlayer = true;
            _player = playerColliders[0].gameObject.transform.parent.transform;
        }
    }

    private void StartIdle()
    {
        _baseBehaviour.StartIdle();
    }

    private void StartHaunt()
    {
        _hauntBehaviour.StartHaunt();
    }

    private void StartChase()
    {
        _chaseBehaviour.StartChase();
    }

    void CheckViewingPlayer()
    {
        if (_healthSystem.IsHacked) return;
        var distanceToPlayer = _player ? Vector3.Distance(transform.position, _player.position) : 0;

        if (distanceToPlayer >= _forgetRange)
        {
            _foundPlayer = false;
        }

        if (PlayerStateObserver.Instance.CurrentState is PlayerStateObserver.PlayerState.Dead)
        {
            _foundPlayer = false;
        }

        Collider[] playerColliders = new Collider[3];
        size = Physics.OverlapSphereNonAlloc(transform.position, _range, playerColliders, _playerLayerMask);
    }

    private void DamagePlayer()
    {
        var playerHealth = _player.GetComponent<HealthSystem>();

        _stabHitSoundEvent.Post(gameObject);
        playerHealth.TakePhysicalDamage(2);
    }

    void Attack()
    {
        StartCoroutine(AttackCooldown(true));
        var front = (_player.position - transform.position).normalized;
        var dot = Vector3.Dot(front, transform.forward);
        var distance = Vector3.Distance(transform.position, _player.position);

        if (distance >= 1.5f || dot < 0.8f)
        {
            _stabSoundEvent.Post(gameObject);
            return;
        }

        DamagePlayer();
    }

    void SpecialAttack()
    {
        StartCoroutine(AttackCooldown(false));
        var front = (_player.position - transform.position).normalized;
        var dot = Vector3.Dot(front, transform.forward);
        var distance = Vector3.Distance(transform.position, _player.position);

        if (distance >= 1.5f || dot < 0.8f)
        {
            _stabSoundEvent.Post(gameObject);
            return;
        }

        DamagePlayer();
    }

    private IEnumerator AttackCooldown(bool basicAttack)
    {
        _startedAttackCooldown = true;

        yield return new WaitForSeconds(0.1f);

        _animator.SetParameterValue("isAttacking", true);

        _attackCooldown = true;
        _currentSpeed = _speed;

        yield return new WaitForSeconds(3);

        _animator.SetParameterValue("isAttacking", false);
        _animator.SetParameterValue("isSpecialAttacking", false);

        _attackCooldown = false;
        _startedAttackCooldown = false;
    }
}
