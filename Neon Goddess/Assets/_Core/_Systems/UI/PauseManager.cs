using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseViewRenderer _renderer;

    private bool _isPaused;
    
    private void OnEnable()
    {
        PlayerInputReader.Instance.EscapePerformed += PausePerformed;

        _renderer.OnResumePressed += ResumeFromButton;
    }

    private void OnDisable()
    {
        PlayerInputReader.Instance.EscapePerformed -= PausePerformed;
        
        _renderer.OnResumePressed -= ResumeFromButton;
    }

    private void PausePerformed()
    {
        if (!_isPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        PlayerInputReader.Instance.DisableInputs();

        Time.timeScale = 0;
        _isPaused = true;
        _renderer.RenderPauseContent();
    }

    private void ResumeFromButton()
    {
        Resume();
    }
    
    private void Resume()
    {
        Time.timeScale = 1;
        _isPaused = false;
        _renderer.HidePauseContent();
        
        PlayerInputReader.Instance.EnableInputs();
    }
}
