using System;
using UnityEngine;

public class InteractableHUDRenderObject : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    
    private IInteractable _interactable;
    private Transform _interactableTransform;
    
    private Canvas _canvas;
    private Camera _mainCamera;

    public IInteractable Interactable => _interactable;
    public bool IsSameInteractable(IInteractable interactable) => interactable == _interactable;

    public void Setup(IInteractable interactable, Transform interactableTransform, Canvas canvas, RectTransform parent)
    {
        _interactable = interactable;
        _interactableTransform = interactableTransform;
        
        _mainCamera = Camera.main;
        _canvas = canvas;
        
        transform.SetParent(parent, false);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.position = _mainCamera.WorldToScreenPoint(_interactableTransform.position);
    }

    private void Update()
    {
        _rectTransform.position = _mainCamera.WorldToScreenPoint(_interactableTransform.position);
    }
}
