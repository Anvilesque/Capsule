using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashCan : MonoBehaviour, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool isPointerDown;
    private float purgeTimer;
    private float purgeDuration;
    private float purgeDelay;
    private bool purgeCompleted;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        isPointerDown = false;
        purgeTimer = 0f;
        purgeDuration = 3f;
        purgeDelay = 1f;
        purgeCompleted = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) Destroy(eventData.pointerDrag);
    }

    private void Update()
    {
        if (!isPointerDown) return;
        else if (purgeCompleted) return;
        else if (purgeTimer >= purgeDuration)
        {
            purgeCompleted = true;
            image.color = new Color(1f, 1f, 1f, 1f);
            DraggableItem draggableItem = FindObjectOfType<DraggableItem>();
            if (draggableItem != null) draggableItem.DestroyAllItems();
            return;
        }
        purgeTimer += Time.deltaTime;
        if (purgeTimer < purgeDelay) return;
        float newColor = Mathf.Max(1 - (purgeTimer - purgeDelay / purgeDuration - purgeDelay), 0f);
        image.color = new Color(newColor, newColor, newColor, 1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        purgeCompleted = false;
        purgeTimer = 0f;
        image.color = new Color(1f, 1f, 1f, 1f);
    }
}
