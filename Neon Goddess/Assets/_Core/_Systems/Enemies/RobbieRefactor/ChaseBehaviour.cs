using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Animations;
using Player;

public class ChaseBehaviour : HuntBehaviour
{

    public void FixedUpdate()
    {
        if (behaviour.stateObserver.CurrentState == PlayerStateObserver.PlayerState.OnDialogue || behaviour.stateObserver.CurrentState == PlayerStateObserver.PlayerState.OnCutscene)
        {
            behaviour._navMeshAgent.isStopped = true;

            return;

        }
        if (behaviour._chasing)
        {
            behaviour._navMeshAgent.isStopped = false;

            if (Vector3.Distance(behaviour._player.position, transform.position) > 1.5f)
            {
                behaviour.Move(behaviour._player.position);
            }
            else
            {
                behaviour._navMeshAgent.SetDestination(transform.position);
            }

            behaviour.RotateTowards(behaviour._player.position);
        }

    }

}
