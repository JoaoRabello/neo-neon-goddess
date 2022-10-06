using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RiggingController : MonoBehaviour
{
    [SerializeField] private Rig _handsRig;
    [SerializeField] private Rig _bodyAimRig;
    [SerializeField] private Rig _weaponStandRig;
    [SerializeField] private Rig _weaponAimRig;
    
    public void TurnOffRigs()
    {
        _handsRig.weight = 0;
        _bodyAimRig.weight = 0;
        _weaponStandRig.weight = 0;
        _weaponAimRig.weight = 0;
    }

    public void TurnOnHandsRigging()
    {
        _handsRig.weight = 1;
    }
    
    public void TurnAimRiggingOn(bool value)
    {
        StartCoroutine(SetRigWeight(_weaponAimRig, value ? 1 : 0));
        // _weaponAimRig.weight = value ? 1 : 0;
        // _bodyAimRig.weight = value ? 1 : 0;
        StartCoroutine(SetRigWeight(_bodyAimRig, value ? 1 : 0));
    }

    private IEnumerator SetRigWeight(Rig rig, float weight)
    {
        yield return null;
        
        rig.weight = weight;
    }
}
