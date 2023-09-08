using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour
{
    [SerializeField] private Slider _masterBusSlider;
    [SerializeField] private Slider _ambienceBusSlider;
    [SerializeField] private Slider _dialoguesBusSlider;
    [SerializeField] private Slider _uiBusSlider;
    [SerializeField] private Slider _notificationsBusSlider;
    [SerializeField] private Slider _musicBusSlider;
    [SerializeField] private Slider _sfxBusSlider;
    
    [SerializeField] private string _masterBusRTPC;
    [SerializeField] private string _ambienceBusRTPC;
    [SerializeField] private string _dialoguesBusRTPC;
    [SerializeField] private string _uiBusRTPC;
    [SerializeField] private string _notificationsBusRTPC;
    [SerializeField] private string _musicBusRTPC;
    [SerializeField] private string _sfxBusRTPC;

    public void SetAllRTPCs()
    {
        AkSoundEngine.SetRTPCValue(_masterBusRTPC, _masterBusSlider.value);
        AkSoundEngine.SetRTPCValue(_ambienceBusRTPC, _ambienceBusSlider.value);
        AkSoundEngine.SetRTPCValue(_dialoguesBusRTPC, _dialoguesBusSlider.value);
        AkSoundEngine.SetRTPCValue(_uiBusRTPC, _uiBusSlider.value);
        AkSoundEngine.SetRTPCValue(_notificationsBusRTPC, _notificationsBusSlider.value);
        AkSoundEngine.SetRTPCValue(_musicBusRTPC, _musicBusSlider.value);
        AkSoundEngine.SetRTPCValue(_sfxBusRTPC, _sfxBusSlider.value);
    }
}