using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverEvent : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] SFXPlayer player;
    public void OnPointerEnter(PointerEventData eventData)
    {
        player.PlaySFX();
    }

}
