using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseViewRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _soundContent;
    public Action OnResumePressed;
    public bool _isSoundActive;

    public void Resume()
    {
        OnResumePressed?.Invoke();
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void RenderPauseContent()
    {
        _content.SetActive(true);
    }
    
    public void HidePauseContent()
    {
        _content.SetActive(false);
    }

    public void showSoundContent()
    {
        _isSoundActive = true;
        _content.SetActive(false);
        _soundContent.SetActive(true);
    }

    public void hideSoundContent()
    {
        _isSoundActive = false;
        _content.SetActive(true);
        _soundContent.SetActive(false);
    }
}
