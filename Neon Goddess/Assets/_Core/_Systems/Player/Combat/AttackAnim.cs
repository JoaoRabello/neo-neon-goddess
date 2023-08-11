using Animations;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnim : MonoBehaviour
{
    [SerializeField] private NewRobbie _robbie;
    
    void AttackAnimate()
    {
        if (PlayerStateObserver.Instance.CurrentState != PlayerStateObserver.PlayerState.Dead)
        {
            _robbie.TryAttack();
        }
    }
}
