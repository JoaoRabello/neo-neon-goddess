using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class WalkPatrolBehaviour : IdleBehaviour
{
    void Update()
    {
        if (_patrol && _idle)
        {
            if (_healthSystem.IsHacked) return;
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
                Hunt();
            }
            else
            {
                WaypointMovement();

                if (size <= 0) return;

                _foundPlayer = true;
                _player = playerColliders[0].gameObject.transform.parent.transform;
            }
        }
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
