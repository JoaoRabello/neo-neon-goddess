using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRootMovement : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private LedgeGrabPrototype _ledgeGrab;
    [SerializeField] private Animator _animator;
    
    void OnAnimatorMove()
    {
        if(_ledgeGrab.IsOnLedge)
            _parent.position += _animator.deltaPosition;
    }
}
