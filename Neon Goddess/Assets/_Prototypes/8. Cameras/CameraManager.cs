using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private GameObject _cameraButtonsParent;
    [SerializeField] private CameraButtonObject _cameraButtonPrefab;
    [SerializeField] private List<CameraButtonObject> _cameraButtons;
    [SerializeField] private List<Camera> _cameras;

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
        var cameras = FindObjectsOfType<Camera>();
        _cameras = new List<Camera>();
        _cameraButtons = new List<CameraButtonObject>();

        bool isFirstCamera = true;
        
        foreach (var foundCamera in cameras)
        {
            if (!isFirstCamera)
            {
                foundCamera.gameObject.SetActive(false);
            }
            else
            {
                isFirstCamera = false;
            }
            
            _cameras.Add(foundCamera);
            var cameraButton = Instantiate(_cameraButtonPrefab, _cameraButtonsParent.transform);
            _cameraButtons.Add(cameraButton);
        }
    }

    private void Start()
    {
        for (var index = 0; index < _cameras.Count; index++)
        {
            var currentCamera = _cameraButtons[index];
            currentCamera.SetIndex(index);
            currentCamera.OnButtonClicked += SelectCamera;
        }
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
    }
}
