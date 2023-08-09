using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class NewRobbie : MonoBehaviour
{
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _specialAttackRange;
    [SerializeField] private float _forgetRange;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;
    
    [Header("Health System")]
    [SerializeField] private RobotHealthSystem _healthSystem;
    
    [Header("Waypoints")]
    [SerializeField] private Transform _waypoint1;
    [SerializeField] private Transform _waypoint2;

    private int _waypointIndex = 1;

    private float _currentSpeed;
    
    private bool _foundPlayer;
    private bool _canMove = true;
    private Transform _player;
    private Vector3 _basePosition;

    private bool _attackCooldown;
    private bool _startedAttackCooldown;

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

    void Update()
    {
        if(_healthSystem.IsHacked) return;
        
        var distanceToPlayer = _player ? Vector3.Distance(transform.position, _player.position) : 0;
        
        if (distanceToPlayer >= _forgetRange)
        {
            _foundPlayer = false;
            WaypointMovement();
            return;
        }
        
        if (_attackCooldown) return;
        
        if (PlayerStateObserver.Instance.CurrentState is PlayerStateObserver.PlayerState.Dead)
        {
            _foundPlayer = false;
        }

        Collider[] playerColliders = new Collider[3];
        var size = Physics.OverlapSphereNonAlloc(transform.position, _range, playerColliders, _playerLayerMask);

        if (_foundPlayer)
        {
            Move(_player.position);
            
            RotateTowards(_player.position);

            Attack();
        }
        else
        {
            WaypointMovement();
            
            if (size <= 0) return;
        
            _foundPlayer = true;
            _player = playerColliders[0].gameObject.transform.parent.transform;
        }
    }

    private void Move(Vector3 desiredPosition)
    {
        if(!_canMove) return;
        
        _navMeshAgent.SetDestination(desiredPosition);

        _animator.SetParameterValue("isWalking", true);
    }

    private void Stop()
    {
        _navMeshAgent.SetDestination(transform.position);
        _animator.SetParameterValue("isWalking", false);
        _animator.SetParameterValue("isAttacking", false);
        _animator.SetParameterValue("isSpecialAttacking", false);
        _animator.PlayAnimation("Idle", 0);
    }

    private void WaypointMovement()
    {
        if (_waypointIndex == 1)
        {
            var xDistance = Mathf.Abs(transform.position.x - _waypoint1.position.x);
            var zDistance = Mathf.Abs(transform.position.z - _waypoint1.position.z);
            if (xDistance <= 0.01f && zDistance <= 0.01f)
            {
                _waypointIndex = 2;
            }
        }
        else
        {
            var xDistance = Mathf.Abs(transform.position.x - _waypoint2.position.x);
            var zDistance = Mathf.Abs(transform.position.z - _waypoint2.position.z);
            if (xDistance <= 0.01f && zDistance <= 0.01f)
            {
                _waypointIndex = 1;
            }
        }

        Vector3 targetPosition = _waypointIndex == 1 ? _waypoint1.position : _waypoint2.position;
            
        Move(targetPosition);
            
        RotateTowards(targetPosition);
    }

    private void Attack()
    {
        if (_startedAttackCooldown) return;

        var distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        if (distanceToPlayer <= _specialAttackRange)
        {
            _currentSpeed = 20;
            StartCoroutine(AttackCooldown(false));
        }
        else if (distanceToPlayer <= _attackRange)
        {
            StartCoroutine(AttackCooldown(true));
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        _navMeshAgent.updateRotation = true;
        // var rotation = (targetPos - transform.position).normalized;
        //
        // transform.forward = rotation;
        // transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private IEnumerator AttackCooldown(bool basicAttack)
    {
        _startedAttackCooldown = true;
        _canMove = false;

        yield return new WaitForSeconds(0.1f);
        
        _navMeshAgent.SetDestination(transform.position);

        _attackCooldown = true;
        _currentSpeed = _speed;

        var playerHealth = _player.GetComponent<HealthSystem>();

        if (basicAttack)
        {
            _animator.SetParameterValue("isAttacking", true);

            playerHealth.TakePhysicalDamage(2);
        }
        else
        {
            _animator.SetParameterValue("isSpecialAttacking", true);
            if (playerHealth.CurrentPhysicalHealth > 1)
            {
                playerHealth.TakeDirectSetPhysicalDamage(1);
            }
            else
            {
                playerHealth.TakePhysicalDamage(2);
            }
        }

        yield return new WaitForSeconds(3);
        
        _animator.SetParameterValue("isAttacking", false);
        _animator.SetParameterValue("isSpecialAttacking", false);
        
        _attackCooldown = false;
        _startedAttackCooldown = false;
        
        _canMove = true;
    }

    private void OnHacked()
    {
        Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _range);
        
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _specialAttackRange);
        
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawSphere(transform.position, _attackRange);
        
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _forgetRange);
    }
}
