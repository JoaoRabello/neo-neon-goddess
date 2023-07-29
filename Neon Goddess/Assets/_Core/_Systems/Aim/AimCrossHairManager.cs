using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AimCrossHairManager : MonoBehaviour
{
    public static AimCrossHairManager Instance;
    
    [SerializeField] private Transform _crossHairsParent;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private AimCrossHairElement _availableCrossHairElement;
    [SerializeField] private AimCrossHairElement _currentCrossHairElement;

    private List<AimCrossHairElement> _availableCrossHairElements = new List<AimCrossHairElement>();

    private Action OnAimCanceled;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CancelAim()
    {
        var tempList = _availableCrossHairElements.ToList();
        foreach (var element in tempList)
        {
            element.Cancel();
            _availableCrossHairElements.Remove(element);
        }
        
        _currentCrossHairElement.Hide();
    }

    public void RenderCrossHair(Transform targetTransform, bool on, bool isCurrentTarget)
    {
        if (on)
        {
            if (!isCurrentTarget)
            {
                var crossHairElement = Instantiate(_availableCrossHairElement, _crossHairsParent);
                crossHairElement.Setup(targetTransform, _canvas);
                
                _availableCrossHairElements.Add(crossHairElement);
            }
            else
            {
                _currentCrossHairElement.gameObject.SetActive(true);
                _currentCrossHairElement.Setup(targetTransform, _canvas);
            }
        }
        else
        {
            if (!isCurrentTarget)
            {
                foreach (var element in _availableCrossHairElements)
                {
                    if(!element.HasSameTransform(targetTransform)) continue;

                    _availableCrossHairElements.Remove(element);
                    
                    break;
                }
            }
            else
            {
                _currentCrossHairElement.Hide();
            }
        }
    }
}
