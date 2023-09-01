using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.AI;
using Animations;

public class ChaseBehaviour : HuntBehaviour
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
    void StartChase()
    {
        if (Vector3.Distance(_player.position, transform.position) > 1.5f)
        {
            Move(_player.position);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
        }

        RotateTowards(_player.position);
    }

    private void RotateTowards(Vector3 targetPos)
    {
        _navMeshAgent.updateRotation = true;
    }

    private void Move(Vector3 desiredPosition)
    {
        if (!_canMove) return;

        _navMeshAgent.SetDestination(desiredPosition);

        _animator.SetParameterValue("isWalking", true);
    }
}
