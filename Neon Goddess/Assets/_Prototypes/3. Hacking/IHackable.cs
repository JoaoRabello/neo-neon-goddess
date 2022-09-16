using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    void Hack();
    void StartHack(float timeToHack);
    void CancelHack();
}
