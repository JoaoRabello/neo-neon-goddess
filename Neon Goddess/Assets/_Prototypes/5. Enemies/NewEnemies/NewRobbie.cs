using System.Collections;
using System.Collections.Generic;
using Animations;
using UnityEngine;

public class NewRobbie : MonoBehaviour
{
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _specialAttackRange;
    [SerializeField] private float _forgetRange;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;
    
    [Header("Waypoints")]
    [SerializeField] private Transform _waypoint1;
    [SerializeField] private Transform _waypoint2;

    private int _waypointIndex = 1;

    private float _currentSpeed;
    
    private bool _foundPlayer;
    private Transform _player;
    private Vector3 _basePosition;

    private bool _attackCooldown;
    private bool _startedAttackCooldown;

    private void Start()
    {
        _currentSpeed = _speed;
        _basePosition = transform.position;
    }

    void Update()
    {
        var distanceToPlayer = _player ? Vector3.Distance(transform.position, _player.position) : 0;
        
        if (distanceToPlayer >= _forgetRange)
        {
            _foundPlayer = false;
            WaypointMovement();
            return;
        }
        
        if (_attackCooldown) return;
        
        Collider[] playerColliders = new Collider[3];
        var size = Physics.OverlapSphereNonAlloc(transform.position, _range, playerColliders, _playerLayerMask);

        if (_foundPlayer)
        {
            Move();
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            
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

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.position, _currentSpeed * Time.deltaTime);
        _animator.SetParameterValue("isWalking", true);
    }

    private void WaypointMovement()
    {
        if (_waypointIndex == 1)
        {
            if (Vector3.Distance(transform.position, _waypoint1.position) <= 0.01f)
            {
                _waypointIndex = 2;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, _waypoint2.position) <= 0.01f)
            {
                _waypointIndex = 1;
            }
        }

        Vector3 targetPosition = _waypointIndex == 1 ? _waypoint1.position : _waypoint2.position;
            
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _currentSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            
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
        transform.forward = (targetPos - transform.position).normalized;
    }

    private IEnumerator AttackCooldown(bool basicAttack)
    {
        _startedAttackCooldown = true;

        yield return new WaitForSeconds(0.1f);

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
