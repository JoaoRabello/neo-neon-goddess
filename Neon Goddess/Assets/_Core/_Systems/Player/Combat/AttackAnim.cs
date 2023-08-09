using Animations;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnim : MonoBehaviour
{
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private PlayerStateObserver _observer;
    // Start is called before the first frame update
    void AttackAnimate()
    {
        if (_observer.CurrentState != PlayerStateObserver.PlayerState.Dead)
        {
            _animator.PlayAnimation("Damage", 0);
        }
    }
}
