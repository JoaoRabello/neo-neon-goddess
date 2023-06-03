using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animatorController;

        public void PlayAnimation()
        {
            
        }

        public void SetParameterValue(string parameterName, bool value)
        {
            _animatorController.SetBool(parameterName, value);
        }
        
        public void SetParameterValue(string parameterName, float value)
        {
            _animatorController.SetFloat(parameterName, value);
        }
        
        public void SetParameterValue(string parameterName, int value)
        {
            _animatorController.SetInteger(parameterName, value);
        }
    }
}
