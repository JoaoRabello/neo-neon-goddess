using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Animations;

public class ChaseBehaviour : HuntBehaviour
{

    public void FixedUpdate()
    {
        if (behaviour._chasing)
        {
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
