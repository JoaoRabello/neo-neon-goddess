using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Halfie : MonoBehaviour
{
    [SerializeField] private Vector3 _initialDirection;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _raycastDistance;
    [SerializeField] private float _speed;
    
    private bool _isFollowingPlayer;
    private Transform _target;
    
    private Vector3 _direction;
    
    void Start()
    {
        _direction = _initialDirection;
    }

    void Update()
    {
        if (!_isFollowingPlayer)
        {
            var ray = new Ray(transform.position, _direction);
            var raycastHit = new RaycastHit();
            var hasHit = Physics.Raycast(ray, out raycastHit, _raycastDistance, _playerLayerMask);
            
            if (hasHit)
            {
                _isFollowingPlayer = true;
                _target = raycastHit.collider.transform;
            }
            else
            {
                _direction = _initialDirection;
            }
        }
        else
        {
            _direction = (_target.position - transform.position).normalized;
        }

        _rigidbody.velocity = new Vector3(_direction.x, _rigidbody.velocity.y, 0) * _speed;
    }
}
