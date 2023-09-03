using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : BaseBehaviour
{
    public void StartIdle()
    {
        _idle = true;
        if (_enemyType == EnemyType.Robbie)
        {
            StartPatrol();
        }
        else if (_enemyType == EnemyType.Eldritch)
        {
            StartLamentation();
        }
    }

    public void EndIdle()
    {
        _idle = false;
        if (_patrol)
        {
            EndPatrol();
        }
        else if (_lamentation)
        {
            EndLamentation();
        }
    }
    public void StartPatrol()
    {
        _patrol = true;
    }

    public void EndPatrol()
    {
        _patrol = false;
    }

    public void StartLamentation()
    {
        _lamentation = true;
    }

    public void EndLamentation()
    { 
        _lamentation = false; 
    }

}
