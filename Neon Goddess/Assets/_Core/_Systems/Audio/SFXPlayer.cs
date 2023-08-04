using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    //TODO: Mudar para evento selecionavel em inspector
    [SerializeField] private string _event;
        
    public void PlaySFX()
    {
        AkSoundEngine.PostEvent(_event, gameObject);
    }
}
