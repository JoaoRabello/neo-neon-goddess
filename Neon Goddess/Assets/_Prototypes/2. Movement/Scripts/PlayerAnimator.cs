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
    
    public void OnCrouch(bool value)
    {
        _animator.SetBool("isCrouching", value);
    }

    public void OnWeaponWield(bool value)
    {
        _animator.SetLayerWeight(1, value ? 1 : 0);
        _animator.SetBool("isAiming", value);
        
        _animator.Play("saque_arma", 1, 0);
    }

    public void OnShoot()
    {
        _animator.SetTrigger("shoot");
    }
}
