using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : MonoBehaviour 
{
    [SerializeField] protected BaseBehaviour behaviour;
    public void StartIdle()
    {
        behaviour._idle = true;
        if (behaviour._enemyType == BaseBehaviour.EnemyType.Robbie)
        {
            StartPatrol();
        }
        else if (behaviour._enemyType == BaseBehaviour.EnemyType.Eldritch)
        {
            StartLamentation();
        }
    }

    public void EndIdle()
    {
        behaviour._idle = false;
        if (behaviour._patrol)
        {
            EndPatrol();
        }
        else if (behaviour._lamentation)
        {
            EndLamentation();
        }
    }
    public void StartPatrol()
    {
        behaviour._patrol = true;
    }

    public void EndPatrol()
    {
        behaviour._patrol = false;
    }

    public void StartLamentation()
    {
        behaviour._lamentation = true;
    }

    public void EndLamentation()
    {
        behaviour._lamentation = false; 
    }

}
