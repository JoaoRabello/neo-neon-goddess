using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchEffect : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    public void PlayGlitch()
    {
        var profile = _volume.sharedProfile;
        profile.TryGet<NTSCEncode>(out var ntsc);
        profile.TryGet<Phosphor>(out var phosphor);

        ntsc.enable.value = true;
        phosphor.enable.value = true;
    }

    public void StopGlitch()
    {
        var profile = _volume.sharedProfile;
        profile.TryGet<NTSCEncode>(out var ntsc);
        profile.TryGet<Phosphor>(out var phosphor);

        ntsc.enable.value = false;
        phosphor.enable.value = false;
    }
}
