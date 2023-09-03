using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Animations;

public class ChaseBehaviour : HuntBehaviour
{

    public void Update()
    {
        if (_chasing)
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
    }

}
