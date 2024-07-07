using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaySoundWithChances : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event _soundEvent;
    [SerializeField] private bool _hasRandomTimeStep;
    [SerializeField] private float _timeStep;
    [SerializeField] private Vector2 _timeStepRange;
    [SerializeField] private float _chance;
    
    [Header("RTPC")]
    [SerializeField] private bool _useRTPC;
    [SerializeField] private AK.Wwise.RTPC _rtpc;
    [SerializeField] private Vector2 _rtpcRandomRange;

    private float timer = 0;

    private void Start()
    {
        TrySetRandomTimeStep();
    }

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
            TrySetRandomTimeStep();
        }
    }

    private void TrySetRandomTimeStep()
    {
        if (!_hasRandomTimeStep) return;

        _timeStep = Random.Range(_timeStepRange.x, _timeStepRange.y);
    }

    private void TryPlaySound()
    {
        var randomDice = Random.Range(1, 101);

        if (randomDice > _chance) return;
        
        if (_useRTPC)
        {
            _rtpc.SetValue(gameObject, Random.Range(_rtpcRandomRange.x, _rtpcRandomRange.y));
        }
        _soundEvent.Post(gameObject);
    }
}
