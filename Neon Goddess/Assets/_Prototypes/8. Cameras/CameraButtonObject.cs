using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraButtonObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _buttonText;
    private int _cameraIndex;
    
    public Action<int> OnButtonClicked;
    
    public void SetIndex(int index)
    {
        _cameraIndex = index;
        _buttonText.text = index.ToString();
    }

    public void SelectCamera()
    {
        OnButtonClicked?.Invoke(_cameraIndex);
    }
}
