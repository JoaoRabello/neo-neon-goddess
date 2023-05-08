using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PagerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _pagerLabel;
    private List<PagerMessage> _receivedMessages = new List<PagerMessage>();
    
    public void ReceiveMessage(PagerMessage message)
    {
        if (_receivedMessages.Contains(message)) return;

        _pagerLabel.SetText(message.Text);
        _receivedMessages.Add(message);
    }
}
