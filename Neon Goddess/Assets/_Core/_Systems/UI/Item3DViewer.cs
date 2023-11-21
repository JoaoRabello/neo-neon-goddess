using Inputs;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Item3DViewer : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] public Item item;
    [SerializeField] GameObject content;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject uiLife;
    [SerializeField] MMFeedbacks _fadeInFeedbacks;
    [SerializeField] MMFeedbacks _warningFeedbacks;

    private Transform _prefab;
    private bool active;
    private bool playedFadeIn;
    private bool isDragging;
  
    void Start()
    {
        PlayerInputReader.Instance.AnyPerformed += Deactivate;
    }

    void Update()
    {
        if (active && !isDragging)
        {
            _prefab.Rotate(0, item.rotationSpeed, 0);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        _prefab.eulerAngles += new Vector3(-eventData.delta.y, -eventData.delta.x);
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void StartInspect()
    {
        StartCoroutine(Activate());
    }
    
    public IEnumerator Activate()
    {
        camera.SetActive(true);
        content.SetActive(true);
        uiLife.SetActive(false);
        Time.timeScale = 0;
        
        _prefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1000), Quaternion.identity);
        
        _fadeInFeedbacks.PlayFeedbacks();
        _warningFeedbacks.PlayFeedbacks();
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        active = true;
        
        yield return new WaitForSecondsRealtime(_fadeInFeedbacks.TotalDuration);

        playedFadeIn = true;
    }
    
    public void Deactivate()
    {
        if (active && playedFadeIn)
        {
            camera.SetActive(false);
            content.SetActive(false);
            Time.timeScale = 1;
            Destroy(_prefab.gameObject);
            active = false;
            uiLife.SetActive(true);
        }
    }
}
