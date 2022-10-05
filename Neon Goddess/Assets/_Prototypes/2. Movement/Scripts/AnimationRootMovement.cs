using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRootMovement : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _centerOfMass;
    
    [SerializeField] private LedgeGrabPrototype _ledgeGrab;
    [SerializeField] private Animator _animator;

    public delegate void WieldAnimationDelegate();
    public event WieldAnimationDelegate OnWieldAnimationComplete;
    public delegate void GetWeaponFromPocketDelegate();
    public event GetWeaponFromPocketDelegate OnGetWeaponFromPocket;
    public delegate void HideAnimationDelegate();
    public event HideAnimationDelegate OnHideAnimationComplete;
    public delegate void HideWeaponOnPocketDelegate();
    public event HideWeaponOnPocketDelegate OnHideWeaponOnPocket;

    private Vector3 _handTargetPosition;
    
    void OnAnimatorMove()
    {
        if (_ledgeGrab.IsOnLedge)
        {
            _parent.position += new Vector3(_animator.deltaPosition.x, _animator.deltaPosition.y, 0);
        }
    }

    public void CompleteWieldAnimation()
    {
        OnWieldAnimationComplete?.Invoke();
    }
    
    public void CompleteHideAnimation()
    {
        OnHideAnimationComplete?.Invoke();
    }

    public void GetWeaponFromPocket()
    {
        OnGetWeaponFromPocket?.Invoke();
    }

    public void HideWeaponOnPocket()
    {
        OnHideWeaponOnPocket?.Invoke();
    }

    public void SetUseRootAnimation(bool value)
    {
        _animator.applyRootMotion = value;
    }

    public void SetHandTargetPosition(Vector3 target)
    {
        _handTargetPosition = target;
    }
}
