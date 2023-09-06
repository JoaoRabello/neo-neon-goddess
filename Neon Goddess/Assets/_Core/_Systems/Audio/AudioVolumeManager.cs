using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour
{
    [SerializeField] private Slider _ambienceBusSlider;
    [SerializeField] private Slider _dialoguesBusSlider;
    [SerializeField] private Slider _uiBusSlider;
    [SerializeField] private Slider _notificationsBusSlider;
    [SerializeField] private Slider _musicBusSlider;
    [SerializeField] private Slider _sfxBusSlider;
    
    [SerializeField] private AK.Wwise.RTPC _ambienceBusRTPC;
    [SerializeField] private AK.Wwise.RTPC _dialoguesBusRTPC;
    [SerializeField] private AK.Wwise.RTPC _uiBusRTPC;
    [SerializeField] private AK.Wwise.RTPC _notificationsBusRTPC;
    [SerializeField] private AK.Wwise.RTPC _musicBusRTPC;
    [SerializeField] private AK.Wwise.RTPC _sfxBusRTPC;

    public void SetAllRTPCs()
    {
        _ambienceBusRTPC.SetGlobalValue(_ambienceBusSlider.value);
        _dialoguesBusRTPC.SetGlobalValue(_dialoguesBusSlider.value);
        _uiBusRTPC.SetGlobalValue(_uiBusSlider.value);
        _notificationsBusRTPC.SetGlobalValue(_notificationsBusSlider.value);
        _musicBusRTPC.SetGlobalValue(_musicBusSlider.value);
        _sfxBusRTPC.SetGlobalValue(_sfxBusSlider.value);
    }
}