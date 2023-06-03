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
        [SerializeField] private Animator _animatorController;

        /// <summary>
        /// Directly plays an animation when called
        /// </summary>
        public void PlayAnimation()
        {
            
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
