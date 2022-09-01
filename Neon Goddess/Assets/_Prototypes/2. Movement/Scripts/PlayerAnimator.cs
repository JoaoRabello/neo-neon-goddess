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
        SetIsOnGround(true);
        SetIsFalling(false);
    }

    public void SetIsOnGround(bool value)
    {
        _animator.SetBool("onGround", value);
    }

    public void SetIsFalling(bool value)
    {
        _animator.SetBool("isFalling", value);
    }

    public void OnMovement(bool value)
    {
        _animator.SetBool("isMoving", value);
    }

    public void OnLedgeGrab(bool value)
    {
        _animator.SetBool("onLedge", value);
    }

    public void OnClimbLedge(bool value)
    {
        _animator.SetBool("isClimbing", value);
    }
}
