using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.position += Vector3.down * 0.1f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.position = originalPos;
    }
}
