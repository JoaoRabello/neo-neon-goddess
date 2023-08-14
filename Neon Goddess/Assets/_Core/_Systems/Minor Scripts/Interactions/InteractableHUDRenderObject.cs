using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractableHUDRenderObject : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;
    
    [Header("Icons")]
    [SerializeField] private Sprite _openDoorIcon;
    [SerializeField] private Sprite _closedDoorIcon;
    [SerializeField] private Sprite _shutDoorIcon;
    [SerializeField] private Sprite _newInteractionIcon;
    [SerializeField] private Sprite _oldInteractionIcon;
    
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

        switch (interactable.GetType())
        {
            case IInteractable.InteractableType.Common:
            case IInteractable.InteractableType.Dialogue:
                _image.sprite = _newInteractionIcon;
                break;
            case IInteractable.InteractableType.Door:
                _image.sprite = _openDoorIcon;
                break;
            case IInteractable.InteractableType.ShutDoor:
                _image.sprite = _shutDoorIcon;
                break;
        }
        
        transform.SetParent(parent, false);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.position = _mainCamera.WorldToScreenPoint(_interactableTransform.position);
    }

    private void Update()
    {
        _rectTransform.position = _mainCamera.WorldToScreenPoint(_interactableTransform.position);
    }
}
