using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using Player;
using UnityEngine;

public class NewHalfie : MonoBehaviour
{
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _forgetRange;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;

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
            Move(_basePosition);
            RotateTowards(_basePosition);
            return;
        }
        
        if (_attackCooldown) return;
        
        Collider[] playerColliders = new Collider[3];
        var size = Physics.OverlapSphereNonAlloc(transform.position, _range, playerColliders, _playerLayerMask);

        if (_foundPlayer)
        {
            Move(_player.position);
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            
            RotateTowards(_player.position);

            Attack();
        }
        else
        {
            if (size <= 0) return;
        
            _foundPlayer = true;
            _player = playerColliders[0].gameObject.transform.parent.transform;
        }
    }

    private void Move(Vector3 destiny)
    {
        transform.position = Vector3.MoveTowards(transform.position, destiny, _currentSpeed * Time.deltaTime);
        _animator.SetParameterValue("isMoving", true);
    }

    private void Attack()
    {
        if (_startedAttackCooldown) return;
        
        if (Vector3.Distance(transform.position, _player.position) <= _attackRange)
        {
            _currentSpeed = 20;
            StartCoroutine(AttackCooldown());
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        transform.forward = (targetPos - transform.position).normalized;
    }

    private IEnumerator AttackCooldown()
    {
        _startedAttackCooldown = true;

        yield return new WaitForSeconds(0.1f);

        _attackCooldown = true;
        _currentSpeed = _speed;
        
        _animator.SetParameterValue("isAttacking", true);

        PlayerStateObserver.Instance.OnAnimationStart();
        CutsceneManager.Instance.PlayCutscene();

        var playerHealth = _player.GetComponent<HealthSystem>();
        if (playerHealth.CurrentPhysicalHealth > 1)
        {
            playerHealth.TakeDirectSetPhysicalDamage(1);
        }
        else
        {
            playerHealth.TakePhysicalDamage(2);
        }
        
        yield return new WaitForSeconds(2);
        
        PlayerStateObserver.Instance.OnAnimationEnd();
        CutsceneManager.Instance.StopCutscene();
        _animator.SetParameterValue("isAttacking", false);
        _animator.SetParameterValue("isWalking", false);
        
        yield return new WaitForSeconds(3);
        
        _attackCooldown = false;
        _startedAttackCooldown = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _range);
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _attackRange);
        
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _forgetRange);
    }
}
