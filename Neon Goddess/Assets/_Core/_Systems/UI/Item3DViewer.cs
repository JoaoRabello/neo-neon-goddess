using Inputs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Item3DViewer : MonoBehaviour, IDragHandler
{
    [SerializeField] public Item item;
    [SerializeField] GameObject content;
    [SerializeField] GameObject camera;
    private Transform _prefab;
    private bool active = false;
    // Start is called before the first frame update
   
    void Start()
    {
        PlayerInputReader.Instance.AnyPerformed += Deactivate;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _prefab.eulerAngles += new Vector3(-eventData.delta.y, -eventData.delta.x);
    }
    // Update is called once per frame
    public void StartInspect()
    {
        StartCoroutine(Activate());
    }
    public IEnumerator Activate()
    {
        camera.SetActive(true);
        content.SetActive(true);
        Time.timeScale = 0;
        _prefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1000), Quaternion.identity);
        yield return new WaitForSecondsRealtime(2);
        active = true; 

    }
    public void Deactivate()
    {
        if (active)
        {
            camera.SetActive(false);
            content.SetActive(false);
            Time.timeScale = 1;
            Destroy(_prefab.gameObject);
            active = false;
        }
        
    }
}
