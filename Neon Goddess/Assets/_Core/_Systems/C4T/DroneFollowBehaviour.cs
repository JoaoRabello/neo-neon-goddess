using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneFollowBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _droneTarget;
    [SerializeField] private float _speed;
    [SerializeField] private float _range;

    private void Update()
    {
        if (Vector3.Distance(transform.position, _droneTarget.position) < _range) return;
        
        transform.Translate((_droneTarget.position - transform.position).normalized * (_speed * Time.deltaTime));
    }
}
