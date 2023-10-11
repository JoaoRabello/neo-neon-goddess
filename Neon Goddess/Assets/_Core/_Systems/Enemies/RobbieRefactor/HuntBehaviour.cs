using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Animations;
using static BaseBehaviour;
using FIMSpace.Basics;
using System;
using UnityEngine.EventSystems;

public class HuntBehaviour : MonoBehaviour
{
    [SerializeField] protected BaseBehaviour behaviour;
    private void FixedUpdate()
    {
        if (behaviour.stateObserver.CurrentState == PlayerStateObserver.PlayerState.OnDialogue || behaviour.stateObserver.CurrentState == PlayerStateObserver.PlayerState.OnCutscene)
        {
            behaviour._navMeshAgent.isStopped = true;
            return;
        }
        if (behaviour._hunting)
        {
            behaviour._navMeshAgent.isStopped = false;

            CheckViewingPlayer();
            if (behaviour._foundPlayer)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, behaviour._player.position);

                if (distanceToPlayer <= behaviour._specialAttackRange)
                {
                    //behaviour._currentSpeed = 20;
                    SpecialAttack();
                }
                else if (distanceToPlayer <= behaviour._attackRange)
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
                if (behaviour._enemyType == EnemyType.Eldritch)
                {
                    StartHaunt();
                }
                else if (behaviour._enemyType == EnemyType.Robbie)
                {
                    behaviour.Idle();
                }

                if (behaviour.size <= 0) return;

                behaviour._foundPlayer = true;
                behaviour._player = behaviour.playerColliders[0].gameObject.transform.parent.transform;
            }
        }
    }

    public void StartHunt()
    {
        behaviour._hunting = true;
        if (behaviour._enemyType == EnemyType.Robbie)
        {
            StartChase();
        }
        else if (behaviour._enemyType == EnemyType.Eldritch)
        {
            StartHaunt();
        }
    }

    public void EndHunt()
    {
        behaviour._hunting = false;
        if (behaviour._chasing)
        {
            EndChase();
        }
        if (behaviour._haunting)
        {
            EndHaunt();
        }
    }

    private void StartHaunt()
    {
        behaviour._haunting = true;
    }

    private void StartChase()
    {
        behaviour._chasing = true;
    }
    private void EndHaunt()
    {
        behaviour._haunting = false;
    }

    private void EndChase()
    {
        behaviour._chasing = false;
    }

    void CheckViewingPlayer()
    {
        if (behaviour._healthSystem.IsHacked) return;
        var distanceToPlayer = behaviour._player ? Vector3.Distance(transform.position, behaviour._player.position) : 0;

        if (distanceToPlayer >= behaviour._forgetRange)
        {
            behaviour._foundPlayer = false;
        }

        if (PlayerStateObserver.Instance.CurrentState is PlayerStateObserver.PlayerState.Dead)
        {
            behaviour._foundPlayer = false;
        }

        Collider[] playerColliders = new Collider[3];
        behaviour.size = Physics.OverlapSphereNonAlloc(transform.position, behaviour._range, playerColliders, behaviour._playerLayerMask);
    }

    private void DamagePlayer()
    {
        var playerHealth = behaviour._player.GetComponent<HealthSystem>();

        behaviour._stabHitSoundEvent.Post(gameObject);
        playerHealth.TakePhysicalDamage(2);
    }

    public void Attack()
    {
        if (behaviour._startedAttackCooldown)
        { return; }
        StartCoroutine(AttackCooldown(true));
    }

    void SpecialAttack()
    {
        if (behaviour._startedAttackCooldown)
        { return; }
        StartCoroutine(AttackCooldown(false));
    }

    public void TryAttack()
    {
        var front = (behaviour._player.position - transform.position).normalized;
        var dot = Vector3.Dot(front, transform.forward);
        var distance = Vector3.Distance(transform.position, behaviour._player.position);

        if (distance >= 1.5f || dot < 0.8f)
        {
            behaviour._stabSoundEvent.Post(gameObject);
            return;
        }

        DamagePlayer();
    }

    private IEnumerator AttackCooldown(bool basicAttack)
    {
        behaviour._startedAttackCooldown = true;

        yield return new WaitForSeconds(0.1f);

        behaviour._animator.SetParameterValue("isAttacking", true);

        behaviour._attackCooldown = true;
        behaviour._currentSpeed = behaviour._speed;

        yield return new WaitForSeconds(3);

        behaviour._animator.SetParameterValue("isAttacking", false);
        behaviour._animator.SetParameterValue("isSpecialAttacking", false);

        behaviour._attackCooldown = false;
        behaviour._startedAttackCooldown = false;
    }
}
