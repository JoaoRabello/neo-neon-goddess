using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    //TODO: Mudar para evento selecionavel em inspector
    [SerializeField] private string _event;
    [SerializeField] private AK.Wwise.Event _akEvent;
        
    public void PlaySFX()
    {
        if (_event == "")
        {
            _akEvent.Post(gameObject);
        }
        else
        {
            AkSoundEngine.PostEvent(_event, gameObject);
        }
    }

    public void AnimationEventPlaySFX()
    {
        if (_event == "")
        {
            _akEvent.Post(gameObject);
        }
        else
        {
            AkSoundEngine.PostEvent(_event, gameObject);
        }
    }
    
    public void PlaySFX(AK.Wwise.Event sfxEvent)
    {
        sfxEvent.Post(gameObject);
    }

    public void StopSFX(AK.Wwise.Event sfxEvent)
    {
        sfxEvent.Stop(gameObject);
    }
}
