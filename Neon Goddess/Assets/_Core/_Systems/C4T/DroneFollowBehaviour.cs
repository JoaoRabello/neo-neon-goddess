using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneFollowBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _droneTarget;
    [SerializeField] private Transform _astridTransform;
    [SerializeField] private Transform _visual;
    [SerializeField] private float _speed;
    [SerializeField] private float _range;

    private void Update()
    {
        if (Vector3.Distance(transform.position, _droneTarget.position) < _range) return;
        
        transform.position = Vector3.MoveTowards(transform.position, _droneTarget.position , _speed * Time.deltaTime);
        _visual.transform.LookAt(_visual.transform.position + _astridTransform.forward);
    }
}
