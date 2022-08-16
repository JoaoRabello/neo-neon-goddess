using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    public void OnJump()
    {
        _animator.SetBool("isJumping", true);
    }

    public void OnGrounded()
    {
        _animator.SetBool("isJumping", false);
    }

    public void OnMovement(bool value)
    {
        _animator.SetBool("isMoving", value);
    }

    public void OnLedgeGrab()
    {
        _animator.SetBool("onLedge", true);
    }

    public void OnClimbLedge()
    {
        _animator.SetBool("isClimbing", true);
    }
}
