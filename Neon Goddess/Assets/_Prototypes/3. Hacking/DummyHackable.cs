using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHackable : MonoBehaviour, IHackable
{
    [SerializeField] private float _timeHacked;
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    private int _waypointIndex;
    
    private bool _isHacking;
    private bool _isHacked;
    
    public void Hack()
    {
        if (_isHacked) return;
        
        _isHacked = true;

        _rigidbody.velocity = Vector3.zero;
        // StopAllCoroutines();
        // StartCoroutine(HackBehaviour());
    }

    public void StartHack(float timeToHack)
    {
        _isHacking = true;
    }

    public void CancelHack()
    {
        _isHacking = false;
    }

    private void Update()
    {
        if (_isHacked) return;
        
        if (Mathf.Abs(Vector2.Distance(transform.position, _waypoints[_waypointIndex].position)) < 0.5f)
        {
            _waypointIndex = _waypointIndex >= _waypoints.Count - 1 ? 0 : _waypointIndex + 1;
        }

        var direction = (_waypoints[_waypointIndex].position - transform.position).normalized;

        _rigidbody.velocity = new Vector3(direction.x, _rigidbody.velocity.y, 0) * _speed;
    }

    public void ClearHack()
    {
         _isHacked = false;
    }

    private IEnumerator HackBehaviour()
    {
        
        yield return new WaitForSeconds(_timeHacked);
        _isHacked = false;
    }
}
