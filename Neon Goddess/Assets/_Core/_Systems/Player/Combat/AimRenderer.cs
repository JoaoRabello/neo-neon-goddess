using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class AimRenderer : MonoBehaviour
    {
        [SerializeField] private AimSystem _aimSystem;
        [SerializeField] private LineRenderer _aimRenderer;
        [SerializeField] private Transform _aimBase;

        private void LateUpdate()
        {
            if (!_aimSystem.IsAiming)
            {
                _aimRenderer.enabled = false;
                return;
            }
            
            _aimRenderer.enabled = true;

            var direction = _aimSystem.CurrentAimingDirection.ToVector3(_aimBase.forward);
            var rendererPositions = new[]
            {
                _aimBase.position,
                _aimBase.position + direction
            };
            _aimRenderer.SetPositions(rendererPositions);
        }
    }
}
