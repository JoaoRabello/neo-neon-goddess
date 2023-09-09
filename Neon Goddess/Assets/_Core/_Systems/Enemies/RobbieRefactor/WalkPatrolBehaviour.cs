using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class WalkPatrolBehaviour : IdleBehaviour
{
    void FixedUpdate()
    {
        if (behaviour._patrol && behaviour._idle)
        {
            if (behaviour._healthSystem.IsHacked) return;
            var distanceToPlayer = behaviour._player ? Vector3.Distance(transform.position, behaviour._player.position) : 0;

            if (distanceToPlayer >= behaviour._forgetRange)
            {
                behaviour._foundPlayer = false;
                WaypointMovement();
                return;
            }

            if (behaviour._attackCooldown) return;

            if (PlayerStateObserver.Instance.CurrentState is PlayerStateObserver.PlayerState.Dead)
            {
                behaviour._foundPlayer = false;
            }

            var size = Physics.OverlapSphereNonAlloc(transform.position, behaviour._range, behaviour.playerColliders, behaviour._playerLayerMask);

            if (behaviour._foundPlayer)
            {
                behaviour.Hunt();
            }
            else
            {
                WaypointMovement();

                if (size <= 0) return;

                behaviour._foundPlayer = true;
                behaviour._player = behaviour.playerColliders[0].gameObject.transform.parent.transform;
            }
        }
    }

    private void WaypointMovement()
    {
        if (behaviour._waypointIndex == 1)
        {
            var xDistance = Mathf.Abs(transform.position.x - behaviour._waypoint1.position.x);
            var zDistance = Mathf.Abs(transform.position.z - behaviour._waypoint1.position.z);
            if (xDistance <= 0.01f && zDistance <= 0.01f)
            {
                behaviour._waypointIndex = 2;
            }
        }
        else
        {
            var xDistance = Mathf.Abs(transform.position.x - behaviour._waypoint2.position.x);
            var zDistance = Mathf.Abs(transform.position.z - behaviour._waypoint2.position.z);
            if (xDistance <= 0.01f && zDistance <= 0.01f)
            {
                behaviour._waypointIndex = 1;
            }
        }

        Vector3 targetPosition = behaviour._waypointIndex == 1 ? behaviour._waypoint1.position : behaviour._waypoint2.position;

        behaviour.Move(targetPosition);
        behaviour.RotateTowards(targetPosition);
    }

}
