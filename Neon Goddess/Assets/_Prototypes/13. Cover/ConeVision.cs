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
            var ray = new Ray(transform.position, (_playerTransform.position - transform.position).normalized);
            
            if (!Physics.Raycast(ray, out var hit, _visionRange)) return;
            
            Debug.Log($"ray hit: {hit.collider.name} vs result: {playerResult.name}");

            _playerIsVisible = hit.collider == playerResult;
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
        
        Gizmos.color = _playerIsVisible ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, (_playerTransform.position - transform.position));
    }
}
