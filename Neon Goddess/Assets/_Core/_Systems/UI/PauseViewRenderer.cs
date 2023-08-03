using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseViewRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _content;

    public Action OnResumePressed;

    public void Resume()
    {
        OnResumePressed?.Invoke();
    }
    
    public void RenderPauseContent()
    {
        _content.SetActive(true);
    }
    
    public void HidePauseContent()
    {
        _content.SetActive(false);
    }
}
