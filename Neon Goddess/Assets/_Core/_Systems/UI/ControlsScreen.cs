using PlayerMovements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class ControlsScreen : MonoBehaviour
{
    [SerializeField] private GameObject _tickObject;
    [SerializeField] private TankMovement _tankScript;
    [SerializeField] private ModernMovement _modernScript;

    private bool _isTank;

    public void changeTick()
    {
        _isTank = !_isTank;
        changeControl();
    }

    public void changeControl()
    {
        _tankScript.enabled = _isTank;
        _modernScript.enabled = !_isTank;
        _tickObject.SetActive(_isTank);
    }
}
