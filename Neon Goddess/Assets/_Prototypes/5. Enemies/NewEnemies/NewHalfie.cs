using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHalfie : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _forgetRange;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;

    private float _currentSpeed;
    
    private bool _foundPlayer;
    private Transform _player;
    private Vector3 _basePosition;

    private bool _attackCooldown;

    private void Start()
    {
        _currentSpeed = _speed;
        _basePosition = transform.position;
    }

    void Update()
    {
        var distanceToPlayer = _player ? Vector3.Distance(transform.position, _player.position) : 0;
        
        if (distanceToPlayer >= _forgetRange)
        {
            _foundPlayer = false;
            transform.position = Vector3.MoveTowards(transform.position, _basePosition, _currentSpeed * Time.deltaTime);
            
            RotateTowards(_basePosition);
            return;
        }
        
        if (_attackCooldown) return;
        
        Collider[] playerColliders = new Collider[3];
        var size = Physics.OverlapSphereNonAlloc(transform.position, _range, playerColliders, _playerLayerMask);

        if (_foundPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.position, _currentSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            
            RotateTowards(_player.position);
            
            if (Vector3.Distance(transform.position, _player.position) <= _attackRange)
            {
                _currentSpeed = 20;
                StartCoroutine(AttackCooldown());
            }
        }
        else
        {
            if (size <= 0) return;
        
            _foundPlayer = true;
            _player = playerColliders[0].gameObject.transform.parent.transform;
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        transform.forward = (targetPos - transform.position).normalized;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        _attackCooldown = true;
        _currentSpeed = _speed;

        yield return new WaitForSeconds(3);
        
        _attackCooldown = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _range);
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _attackRange);
        
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _forgetRange);
    }
}
