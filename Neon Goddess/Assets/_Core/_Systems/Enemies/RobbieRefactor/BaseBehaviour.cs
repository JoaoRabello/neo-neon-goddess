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
    [SerializeField] public float _rotationSpeed;
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

    public enum ParalysisResistance
    {
        ParalysisResistanceCommon,  
        ParalysisResistanceI,
        ParalysisResistanceII
    }

    public ParalysisResistance _paralysisResistance;
    public float _resistanceTime;
    public int _resistanceChances;
    public bool _paralysed;
    public bool _forcedResistance;
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
        var i = Random.Range(0, 100);
        Debug.Log(i);
        if (i < 25 || _forcedResistance)
        {
            _paralysisResistance = (ParalysisResistance)1;
            _resistanceChances = 1;
            var time = Random.Range(1f, 11f);
            _resistanceTime = time;
            var j = Random.Range(0, 100);
            if (j == 0)
            {
                _paralysisResistance = (ParalysisResistance)2;
                _resistanceChances = 2;
            }
        }
        else
        {
            _paralysisResistance = (ParalysisResistance)0;
            _resistanceChances = 0;
        }

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
        _paralysed = true;
        _navMeshAgent.SetDestination(transform.position);
        _animator._animatorController.speed = 0f;
        StartCoroutine(TryResist());
    }

    public void RotateTowards(Vector3 targetPos)
    {
        _navMeshAgent.updateRotation = true;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_player.transform.position - transform.position), _rotationSpeed * Time.fixedDeltaTime);
    }

    public void Move(Vector3 desiredPosition)
    {
        if (!_canMove) return;

        _navMeshAgent.SetDestination(desiredPosition);

        _animator.SetParameterValue("isWalking", true);
    }

    public IEnumerator TryResist()
    {
        if (_paralysisResistance != ParalysisResistance.ParalysisResistanceCommon) 
        {
            if (_resistanceChances != 0 && _paralysed == true)
            {
                yield return new WaitForSeconds(_resistanceTime);
                var shot = Random.Range(0, 2);
                if (shot == 1)
                {
                    Deparalyse();
                }
                _resistanceChances -= 1;
                Debug.Log(_resistanceChances);
            }
        }

    }

    public void Deparalyse()
    {
        _paralysed = false;
        _animator._animatorController.speed = 1f;
        Idle();
    }
}
