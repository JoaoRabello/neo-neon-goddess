using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableHUDManager : MonoBehaviour
{
    public static InteractableHUDManager Instance;
    [SerializeField] private List<InteractableHUDRenderObject> _renderObjectPool = new List<InteractableHUDRenderObject>();
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _parent;
    [SerializeField] private PlayerStateObserver _playerStateObserver;

    private Dictionary<IInteractable, InteractableHUDRenderObject> _renderObjectsByInteractable = new Dictionary<IInteractable, InteractableHUDRenderObject>();

    private int _currentActiveRenderObjectCount;
    private bool _interactionsOn;
    
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

    private void OnEnable()
    {
        CameraManager.Instance.OnAnyCameraChange += ClearAllInteractables;
    }
    
    private void OnDisable()
    {
        CameraManager.Instance.OnAnyCameraChange -= ClearAllInteractables;
    }

    private void ClearAllInteractables()
    {
        var list = _renderObjectsByInteractable.ToList();
        
        foreach (var dictionaryValuePair in list)
        {
            _renderObjectsByInteractable[dictionaryValuePair.Key].gameObject.SetActive(false);
            _renderObjectsByInteractable.Remove(dictionaryValuePair.Key);
            _renderObjectPool.Sort(ByActive);
        
            dictionaryValuePair.Key.OnInteractUpdateIcon -= UpdateVisual;
            dictionaryValuePair.Key.OnStateChangeUpdateIcon -= UpdateVisual;
        
            _currentActiveRenderObjectCount--;
        }
    }

    private void Update()
    {
        if (_interactionsOn && (_playerStateObserver._currentState == PlayerStateObserver.PlayerState.OnCamera || _playerStateObserver._currentState == PlayerStateObserver.PlayerState.OnCutscene))
        {
            foreach(var obj in _renderObjectPool)
            {
                obj._image.enabled = false;
            }
            _interactionsOn = false;
        }
        else if (!_interactionsOn && _playerStateObserver._currentState != PlayerStateObserver.PlayerState.OnCamera && _playerStateObserver._currentState != PlayerStateObserver.PlayerState.OnCutscene)
        {
            foreach (var obj in _renderObjectPool)
            {
                obj._image.enabled = true;
            }
            _interactionsOn = true;
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
