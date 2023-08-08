using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCrossHairElement : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private Canvas _canvas;
    private IHackable _followHackable;
    private Camera _mainCamera;

    public bool HasSameHackable(IHackable hackable) => hackable == _followHackable;
    
    public void Setup(IHackable hackableToFollow, Canvas canvas)
    {
        _followHackable = hackableToFollow;
        var position = hackableToFollow.GetTorsoPosition();

        _mainCamera = Camera.main;
        _canvas = canvas;
        
        transform.SetParent(_canvas.transform, false);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.position = _mainCamera.WorldToScreenPoint(position);
    }

    public void Cancel()
    {
        _followHackable = null;
        Destroy(gameObject);
    }

    public void Hide()
    {
        _followHackable = null;
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        if(_followHackable == null) return;
        
        var position = _followHackable.GetTorsoPosition();
        _rectTransform.position = _mainCamera.WorldToScreenPoint(position);
    }
}
