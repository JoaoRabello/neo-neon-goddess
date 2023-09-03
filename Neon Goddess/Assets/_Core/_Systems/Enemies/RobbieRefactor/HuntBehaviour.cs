using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Animations;
using static BaseBehaviour;
using FIMSpace.Basics;
using System;

public class HuntBehaviour : BaseBehaviour
{       
    private void Update()
    {
        if (_hunting)
        {
            CheckViewingPlayer();
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
                else if (_enemyType == EnemyType.Robbie)
                {
                    Idle();
                }

                if (size <= 0) return;

                _foundPlayer = true;
                _player = playerColliders[0].gameObject.transform.parent.transform;
            }
        }
    }

    public void StartHunt()
    {
        _hunting = true;
        if (_enemyType == EnemyType.Robbie)
        {
            StartChase();
        }
        else if (_enemyType == EnemyType.Eldritch)
        {
            StartHaunt();
        }
    }

    public void EndHunt()
    {
        _hunting = true;
        if (_chasing)
        {
            EndChase();
        }
        if (_haunting)
        {
            EndHaunt();
        }
    }

    private void StartHaunt()
    {
        _haunting = true;
    }

    private void StartChase()
    {
        _chasing = true;
    }
    private void EndHaunt()
    {
        _haunting = false;
    }

    private void EndChase()
    {
        _chasing = false;
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
