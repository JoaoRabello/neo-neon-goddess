using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PagerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _bigPagerLabel;
    [SerializeField] private TMP_Text _pagerFrequencyLabel;
    [SerializeField] private TMP_Text _pagerLabel;
    [SerializeField] private PagerMessageRenderObject _messageRenderObjectPrefab;
    [SerializeField] private Transform _bigPagerMessageButtonsParent;
    [SerializeField] private GameObject _bigPagerContent;
    [SerializeField] private int _maxCharacterCount;
    private List<PagerMessage> _receivedMessages = new List<PagerMessage>();
    private List<GameObject> _messageRenderObjects = new List<GameObject>();
    
    public void ReceiveMessage(PagerMessage message)
    {
        if (_receivedMessages.Contains(message)) return;

        RenderMessage(message);
    }

    private void RenderMessage(PagerMessage message)
    {
        _pagerFrequencyLabel.SetText($"{message.SenderFrequency}");
        _pagerLabel.SetText($"{message.Text[.._maxCharacterCount]}...");
        _receivedMessages.Add(message);
    }
    
    private void RenderBigMessage(PagerMessage message)
    {
        _bigPagerLabel.SetText($"{message.Text}");
        message.Read = true;
    }

    public void ShowBigPager()
    {
        foreach (var message in _receivedMessages)
        {
            var renderObject = Instantiate(_messageRenderObjectPrefab, _bigPagerMessageButtonsParent);
            renderObject.Setup(message);

            renderObject.MessageSelected += OnMessageSelected;
            
            _messageRenderObjects.Add(renderObject.gameObject);
        }
        
        _bigPagerContent.SetActive(true);
    }

    public void CloseBigPager()
    {
        var allRenderObjects = _messageRenderObjects.ToList();
        foreach (var renderObject in allRenderObjects)
        {
            Destroy(renderObject);
        }
        
        _bigPagerContent.SetActive(false);
    }

    private void OnMessageSelected(PagerMessage message)
    {
        RenderBigMessage(message);
    }
}
