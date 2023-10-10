using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private List<CameraButtonObject> _cameraButtons;
    [SerializeField] private List<Camera> _cameras;
    [SerializeField] private Camera _firstCamera;

    private Camera _lastActiveRoomCamera;
    public Camera LastActiveRoomCamera => _lastActiveRoomCamera;

    public Action<Camera> OnActiveRoomCameraChange;
    
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnEnable()
    {
        var cameras = FindObjectsOfType<Camera>();
        _cameras = new List<Camera>();

        bool isFirstCamera = true;
        
        foreach (var foundCamera in cameras)
        {
            foundCamera.gameObject.SetActive(false);
            
            _cameras.Add(foundCamera);
        }
        
        _cameras.Add(_firstCamera);
        _firstCamera.gameObject.SetActive(true);

        _lastActiveRoomCamera = _firstCamera;
    }

    private void SelectCamera(int index)
    {
        for (int i = 0; i < _cameras.Count; i++)
        {
            _cameras[i].gameObject.SetActive(i == index);
        }
    }

    public void SelectCamera(Camera cameraToSelect)
    {
        foreach (var searchCamera in _cameras)
        {
            searchCamera.gameObject.SetActive(searchCamera.Equals(cameraToSelect));
        }
        
        _lastActiveRoomCamera = cameraToSelect;
        OnActiveRoomCameraChange?.Invoke(_lastActiveRoomCamera);
    }
    
    public void TurnOffRoomCamera()
    {
        _lastActiveRoomCamera.gameObject.SetActive(false);
    }

    public void TurnOnLastRoomCamera()
    {
        _lastActiveRoomCamera.gameObject.SetActive(true);
    }
}
