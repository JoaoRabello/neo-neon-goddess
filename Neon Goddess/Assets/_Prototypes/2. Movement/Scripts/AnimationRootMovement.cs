using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRootMovement : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _centerOfMass;
    
    [SerializeField] private LedgeGrabPrototype _ledgeGrab;
    [SerializeField] private Animator _animator;
    
    void OnAnimatorMove()
    {
        if (_ledgeGrab.IsOnLedge)
        {
            _parent.position += new Vector3(_animator.deltaPosition.x, _animator.deltaPosition.y, 0);
        }
    }

    public void SetUseRootAnimation(bool value)
    {
        _animator.applyRootMotion = value;
    }
}
