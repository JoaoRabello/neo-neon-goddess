using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    /// <summary>
    /// Component that controls animators due to external calls
    /// </summary>
    public class CharacterAnimator : MonoBehaviour
    {
        [Tooltip("Animator controller that controls the current character animations")]
        [SerializeField] public Animator _animatorController;

        public Vector3 DeltaPosition => _animatorController.deltaPosition;
        
        /// <summary>
        /// Directly plays an animation when called
        /// </summary>
        /// <param name="animationName">Name of the desired animation</param>
        /// <param name="time">Start time</param>
        public void PlayAnimation(string animationName, int time)
        {
            _animatorController.Play(animationName, time);
        }

        public void PlayAndOnAnimationEndCallback(string animationName, Action callback)
        {
            PlayAnimation(animationName, 0);
            StartCoroutine(WaitForAnimationToEndByName(animationName, callback));
        }
        
        public void PlayAndOnAnimationChangeCallback(string animationName, Action callback)
        {
            PlayAnimation(animationName, 0);
            StartCoroutine(WaitForAnimationByNameToChange(animationName, callback));
        }

        public void OnCurrentAnimationEndCallback(Action callback)
        {
            StartCoroutine(WaitForCurrentAnimationToEnd(callback));
        }

        private IEnumerator WaitForAnimationToEndByName(string animationName, Action callback)
        {
            while (!_animatorController.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                yield return null;
            }

            while (_animatorController.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.99f)
            {
                if (!_animatorController.GetCurrentAnimatorStateInfo(0).IsName(animationName))
                {
                    break;
                }
                yield return null;
            }
            callback?.Invoke();
        }
        
        private IEnumerator WaitForAnimationByNameToChange(string animationName, Action callback)
        {
            while (!_animatorController.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                yield return null;
            }
            
            while (_animatorController.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                yield return null;
            }

            callback?.Invoke();
        }
        
        private IEnumerator WaitForCurrentAnimationToEnd(Action callback)
        {
            var test = _animatorController.GetCurrentAnimatorStateInfo(0);
            while (_animatorController.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.99f)
            {
                yield return null;
            }
            
            callback?.Invoke();
        }

        /// <summary>
        /// Sets the parameter to the desired value
        /// </summary>
        /// <param name="parameterName">Animator parameter name</param>
        /// <param name="value">Value to pass as bool</param>
        public void SetParameterValue(string parameterName, bool value)
        {
            _animatorController.SetBool(parameterName, value);
        }
        
        /// <summary>
        /// Sets the parameter to the desired value
        /// </summary>
        /// <param name="parameterName">Animator parameter name</param>
        /// <param name="value">Value to pass as float</param>
        public void SetParameterValue(string parameterName, float value)
        {
            _animatorController.SetFloat(parameterName, value);
        }
        
        /// <summary>
        /// Sets the parameter to the desired value
        /// </summary>
        /// <param name="parameterName">Animator parameter name</param>
        /// <param name="value">Value to pass as int</param>
        public void SetParameterValue(string parameterName, int value)
        {
            _animatorController.SetInteger(parameterName, value);
        }
    }
}
