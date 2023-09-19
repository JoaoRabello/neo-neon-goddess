using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneWithFade : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeTime;
    
    public void FadeInToScene(int buildIndex)
    {
        StartCoroutine(FadeIn(buildIndex));
    }

    private IEnumerator FadeIn(int buildIndex)
    {
        var timer = 0f;
        var color = _fadeImage.color;
        while (timer < _fadeTime)
        {
            timer += Time.deltaTime;

            _fadeImage.color = new Color(color.r, color.g, color.b, timer / _fadeTime);

            yield return null;
        }
        
        SceneManager.LoadScene(buildIndex);
    }
}
