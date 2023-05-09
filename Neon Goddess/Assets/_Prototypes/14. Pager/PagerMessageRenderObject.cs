using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PagerMessageRenderObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _frequencyLabel;
    [SerializeField] private GameObject _readAlert;
    
    private PagerMessage _message;
    public Action<PagerMessage> MessageSelected;
    
    public void Setup(PagerMessage message)
    {
        _message = message;

        _frequencyLabel.SetText($"{_message.SenderFrequency}");
        _readAlert.SetActive(!_message.Read);
    }

    public void SelectMessage()
    {
        MessageSelected?.Invoke(_message);
        
        _readAlert.SetActive(false);
    }
}
