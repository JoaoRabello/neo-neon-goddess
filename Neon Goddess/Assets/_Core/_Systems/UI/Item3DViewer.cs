using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class Item3DViewer : MonoBehaviour, IDragHandler
{
    [SerializeField] Item item;
    private Transform _prefab;
    // Start is called before the first frame update
    void Start()
    {
        if (_prefab != null)
        { Destroy(_prefab.gameObject); }    
       _prefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1000), Quaternion.identity);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _prefab.eulerAngles += new Vector3(-eventData.delta.y, -eventData.delta.x);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
