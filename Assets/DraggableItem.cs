using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    private Camera gameCam;
    private Canvas canvasBookshelf;

    private void Start()
    {
        canvasBookshelf = new List<Canvas>(FindObjectsOfType<Canvas>()).Find(x=> x.name.Contains("Bookshelf"));
        gameCam = new List<Camera>(FindObjectsOfType<Camera>()).Find(x=> x.name.Contains("Bookshelf"));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag.");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("On drag.");
        ChangePos();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag.");
        ChangePos();
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    private void ChangePos()
    {
        if (canvasBookshelf == null) {}
        else switch (canvasBookshelf.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            {
                Vector3 mousePosition = Input.mousePosition;
                transform.position = mousePosition;
                break;
            }
            case RenderMode.ScreenSpaceCamera:
            {
                Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, mousePos.y, 150f);
                break;
            }
        }
    }
}
