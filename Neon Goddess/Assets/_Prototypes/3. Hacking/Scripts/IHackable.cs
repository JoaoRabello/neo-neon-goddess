using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    void TakeHackShot(int damageAmount);
    void Hack();
    void StartHack(float timeToHack);
    void CancelHack();
}
