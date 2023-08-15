using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractableHUDRenderObject : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;
    [SerializeField] private Animator _animator;
    
    [Header("Icons")]
    [SerializeField] private string _openDoorAnimName;
    [SerializeField] private Sprite _openDoorIcon;
    [SerializeField] private string _closedDoorAnimName;
    [SerializeField] private Sprite _closedDoorIcon;
    [SerializeField] private string _shutDoorAnimName;
    [SerializeField] private Sprite _shutDoorIcon;
    [SerializeField] private string _newInteractionAnimName;
    [SerializeField] private Sprite _newInteractionIcon;
    [SerializeField] private string _oldInteractionAnimName;
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
                _image.sprite = interactable.HasInteractedOnce() ? _oldInteractionIcon : _newInteractionIcon;
                _animator.Play(interactable.HasInteractedOnce() ? _oldInteractionAnimName : _newInteractionAnimName);
                break;
            case IInteractable.InteractableType.Door:
                if (interactable.IsLocked())
                {
                    _image.sprite = _closedDoorIcon;
                    _animator.Play(_closedDoorAnimName);
                }
                else
                {
                    _image.sprite = _openDoorIcon;
                    _animator.Play(_openDoorAnimName);
                }
                break;
            case IInteractable.InteractableType.ShutDoor:
                _image.sprite = _shutDoorIcon;
                _animator.Play(_shutDoorAnimName);
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
