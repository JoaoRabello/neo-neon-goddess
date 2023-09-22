using System;
using UnityEngine;

public class StopAllSoundsOnLoadScene : MonoBehaviour
{
    void Awake()
    {
        AkSoundEngine.StopAll();
    }

    private void OnDisable()
    {
        AkSoundEngine.StopAll();
    }
}
