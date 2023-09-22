using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int _openingSceneIndex;
    
    public void Play()
    {
        SceneManager.LoadScene(_openingSceneIndex, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
