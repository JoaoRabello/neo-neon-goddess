using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHUDManager : MonoBehaviour
{
    public static InteractableHUDManager Instance;
    [SerializeField] private List<InteractableHUDRenderObject> _renderObjectPool = new List<InteractableHUDRenderObject>();
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _parent;

    private Dictionary<IInteractable, InteractableHUDRenderObject> _renderObjectsByInteractable = new Dictionary<IInteractable, InteractableHUDRenderObject>();

    private int _currentActiveRenderObjectCount;
    
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
    
    public void SetupRenderObjects()
    {
        
    }

    private void UpdateVisual(IInteractable interactable)
    {
        switch (interactable.GetInteractableType())
        {
            case IInteractable.InteractableType.Common:
            case IInteractable.InteractableType.Dialogue:
                if(!interactable.HasInteractedOnce()) return;
                _renderObjectsByInteractable[interactable].ChangeCommonIcon();
                break;
            case IInteractable.InteractableType.Door:
                if(interactable.IsLocked()) return;
                _renderObjectsByInteractable[interactable].ChangeDoorIcon();
                break;
        }
    }

    public void AddObject(IInteractable interactable, Transform interactableTransform)
    {
        if(_renderObjectsByInteractable.ContainsKey(interactable)) return;

        _renderObjectPool[_currentActiveRenderObjectCount].gameObject.SetActive(true);
        _renderObjectPool[_currentActiveRenderObjectCount].Setup(interactable, interactableTransform, _canvas, _parent);
        _renderObjectsByInteractable[interactable] = _renderObjectPool[_currentActiveRenderObjectCount];

        interactable.OnInteractUpdateIcon += UpdateVisual;
        interactable.OnStateChangeUpdateIcon += UpdateVisual;
        
        _currentActiveRenderObjectCount++;
    }

    public void RemoveObject(IInteractable interactable)
    {
        if(!_renderObjectsByInteractable.ContainsKey(interactable)) return;

        _renderObjectsByInteractable[interactable].gameObject.SetActive(false);
        _renderObjectsByInteractable.Remove(interactable);
        _renderObjectPool.Sort(ByActive);
        
        interactable.OnInteractUpdateIcon -= UpdateVisual;
        interactable.OnStateChangeUpdateIcon -= UpdateVisual;
        
        _currentActiveRenderObjectCount--;
    }
    
    private int ByActive(InteractableHUDRenderObject a, InteractableHUDRenderObject b)
    {
        var activeA = a.isActiveAndEnabled;
        var activeB = b.isActiveAndEnabled;
        return activeA.CompareTo(activeB);
    }
}
