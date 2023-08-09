using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private PlayerStateObserver _observer;
    [SerializeField] private GameObject _button;
    private Image _image;
    private bool started;

    // Update is called once per frame

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (_observer.CurrentState == PlayerStateObserver.PlayerState.Dead && !started)
        {
            StartCoroutine(FadeImg());
            started = true;
        }
    }

    IEnumerator FadeImg()
    {
        yield return new WaitForSeconds(5);
        for (float i = 0; i <= 1.2f; i += Time.deltaTime)
        {
            _image.color = new Color(0, 0, 0, i);
            yield return null;
        }
        _button.SetActive(true);
    }
}
