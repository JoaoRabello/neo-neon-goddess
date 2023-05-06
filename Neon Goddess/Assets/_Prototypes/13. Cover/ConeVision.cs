using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeVision : MonoBehaviour
{
    [SerializeField] private float _visionRange;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private LayerMask _playerMask;

    private Transform _playerTransform;
    private bool _playerIsVisible;
    
    void Update()
    {
        RotateAroundItself();
        TryFindPlayer();
    }

    private void RotateAroundItself()
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * _rotationSpeed);
    }

    private void TryFindPlayer()
    {
        var myPos = transform.position;
        var results = new Collider[2];
        var size = Physics.OverlapSphereNonAlloc(myPos, _visionRange, results, _playerMask);

        if (size <= 0)
        {
            _playerTransform = null;
            return;
        }

        var playerResult = results[0];

        _playerTransform = playerResult.transform;
        var playerPos = playerResult.transform.position;
        var directionToPlayer = (playerPos - myPos).normalized;
        var dotResult = Vector3.Dot(transform.forward, directionToPlayer);

        if (dotResult >= 0.5f)
        {
            var playerCover = _playerTransform.GetComponentInParent<PlayerCover>();
            var coverHitboxesList = playerCover.HitBoxes;
            
            foreach (var hitBox in coverHitboxesList)
            {
                var direction = (hitBox.transform.position - transform.position).normalized;
                var ray = new Ray(transform.position, direction);
            
                if (!Physics.Raycast(ray, out var hit, _visionRange)) continue;
                if (hit.collider != hitBox) continue;
                    
                Debug.DrawRay(transform.position, hitBox.transform.position - transform.position, Color.green);

                _playerIsVisible = true;
                return;
            }

            _playerIsVisible = false;
        }
        else
        {
            _playerIsVisible = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, _visionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _visionRange);

        if (!_playerTransform) return;
        if (_playerIsVisible) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (_playerTransform.position - transform.position));
    }
}
