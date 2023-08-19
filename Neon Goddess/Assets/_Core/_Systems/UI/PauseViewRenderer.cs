using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseViewRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _content;

    public Action OnResumePressed;

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
}
