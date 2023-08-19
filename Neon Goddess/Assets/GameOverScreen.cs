using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void mainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene("Newblockout", LoadSceneMode.Single);
    }
}
