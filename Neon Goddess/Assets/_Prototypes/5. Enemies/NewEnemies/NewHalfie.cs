using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHalfie : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;

    private bool _foundPlayer;
    private Transform _player;

    void Update()
    {
        Collider[] playerColliders = new Collider[3];
        var size = Physics.OverlapSphereNonAlloc(transform.position, _range, playerColliders, _playerLayerMask);

        if (_foundPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.position, _speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);

            transform.forward = (_player.position - transform.position).normalized;
        }
        else
        {
            if (size <= 0) return;
        
            _foundPlayer = true;
            _player = playerColliders[0].gameObject.transform.parent.transform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _range);
    }
}
