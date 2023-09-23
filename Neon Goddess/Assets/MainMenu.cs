using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MMF_Player _fadeEffect;
    [SerializeField] private SFXPlayer _startGameSFX;
    [SerializeField] private float _timeToWaitAfterFade;
    [SerializeField] private int _openingSceneIndex;
    
    public void Play()
    {
        StartCoroutine(WaitForFade());
    }

    private IEnumerator WaitForFade()
    {
        _fadeEffect.PlayFeedbacks();

        while (_fadeEffect.ElapsedTime < _fadeEffect.TotalDuration)
        {
            yield return null;
        }
        
        _startGameSFX.PlaySFX();

        yield return new WaitForSeconds(_timeToWaitAfterFade);
        
        SceneManager.LoadScene(_openingSceneIndex, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
