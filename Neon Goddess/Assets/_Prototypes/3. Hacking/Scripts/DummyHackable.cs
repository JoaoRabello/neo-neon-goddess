using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyHackable : MonoBehaviour, IHackable
{
    [Header("Combat")] 
    [SerializeField] private int _systemResistance;
    [SerializeField] private int _systemArmor;

    private int _currentSystemResistance;

    [Header("UI")] 
    [SerializeField] private Slider _systemResistanceBar;
    [SerializeField] private Light _hackingLight;
    [SerializeField] private List<Color> _hackingColors = new List<Color>();
    
    [Header("Movement")] 
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    private int _waypointIndex;

    private bool _isHacking;
    private bool _isHacked;

    private void Start()
    {
        _currentSystemResistance = _systemResistance;
            
        _hackingLight.color = _hackingColors[0];
    }

    public void SetAsPossibleTarget(bool value)
    {
        throw new NotImplementedException();
    }

    public void SetAsCurrentTarget(bool value)
    {
        throw new NotImplementedException();
    }

    public void TakeHackShot(int damageAmount)
    {
        if (_isHacked) return;
        
        var damageTaken = damageAmount - _systemArmor;

        if (damageTaken >= _currentSystemResistance)
        {
            _currentSystemResistance = 0;
            
            Hack();
        }
        else
        {
            _currentSystemResistance -= damageTaken;
            
            _hackingLight.color = _hackingColors[1];
        }
        
        _systemResistanceBar.value = (float)_currentSystemResistance / _systemResistance;
    }

    public void Hack()
    {
        if (_isHacked) return;
        
        _isHacked = true;

        _rigidbody.velocity = Vector3.zero;
        
        _hackingLight.color = _hackingColors[2];
        _systemResistanceBar.gameObject.SetActive(false);
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

    public Vector3 GetHeadPosition()
    {
        throw new NotImplementedException();
    }

    public Vector3 GetTorsoPosition()
    {
        throw new NotImplementedException();
    }

    public Vector3 GetLegsPosition()
    {
        throw new NotImplementedException();
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
        yield return new WaitForSeconds(0);
        _isHacked = false;
    }
}
