using Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseBehaviour : MonoBehaviour
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
    [SerializeField] private HuntBehaviour _huntBehaviour;
    [SerializeField] private IdleBehaviour _idleBehaviour;

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
        _huntBehaviour.StartHunt();
    }
    private void OnHacked()
    {
        Stop();
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
