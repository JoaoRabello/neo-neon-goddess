using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    void SetAsPossibleTarget(bool value);
    void SetAsCurrentTarget(bool value);
    void TakeHackShot(int damageAmount);
    void Hack();
    void StartHack(float timeToHack);
    void CancelHack();

    Vector3 GetHeadPosition();
    Vector3 GetTorsoPosition();
    Vector3 GetLegsPosition();
}
