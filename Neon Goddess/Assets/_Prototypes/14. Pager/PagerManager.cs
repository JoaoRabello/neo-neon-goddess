using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PagerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _pagerFrequencyLabel;
    [SerializeField] private TMP_Text _pagerLabel;
    [SerializeField] private int _maxCharacterCount;
    private List<PagerMessage> _receivedMessages = new List<PagerMessage>();
    
    public void ReceiveMessage(PagerMessage message)
    {
        if (_receivedMessages.Contains(message)) return;

        _pagerFrequencyLabel.SetText($"{message.SenderFrequency}");
        _pagerLabel.SetText($"{message.Text[.._maxCharacterCount]}...");
        _receivedMessages.Add(message);
    }
}
