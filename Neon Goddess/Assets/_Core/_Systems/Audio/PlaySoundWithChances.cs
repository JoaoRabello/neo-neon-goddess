using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundWithChances : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event _soundEvent;
    [SerializeField] private float _timeStep;
    [SerializeField] private float _chance;

    private float timer = 0;
    
    private void Update()
    {
        if (timer < _timeStep)
        {
            timer += Time.deltaTime;
        }
        else
        {
            TryPlaySound();

            timer = 0;
        }
    }

    private void TryPlaySound()
    {
        var randomDice = Random.Range(1, 101);

        if (randomDice <= _chance)
        {
            _soundEvent.Post(gameObject);
        }
    }
}
