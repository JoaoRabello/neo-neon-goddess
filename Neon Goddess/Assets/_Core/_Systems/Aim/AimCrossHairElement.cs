using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCrossHairElement : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private Canvas _canvas;
    private Transform _followTransform;
    private Camera _mainCamera;

    public bool HasSameTransform(Transform transform) => transform == _followTransform;
    
    public void Setup(Transform transformToFollow, Canvas canvas)
    {
        _followTransform = transformToFollow;
        var position = new Vector3(_followTransform.position.x, _followTransform.position.y + 1, _followTransform.position.z);

        _mainCamera = Camera.main;
        _canvas = canvas;
        
        transform.SetParent(_canvas.transform, false);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.position = _mainCamera.WorldToScreenPoint(position);
    }

    public void Cancel()
    {
        _followTransform = null;
        Destroy(gameObject);
    }

    public void Hide()
    {
        _followTransform = null;
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        if(!_followTransform) return;
        
        var position = new Vector3(_followTransform.position.x, _followTransform.position.y, _followTransform.position.z);
        _rectTransform.position = _mainCamera.WorldToScreenPoint(position);
    }
}
