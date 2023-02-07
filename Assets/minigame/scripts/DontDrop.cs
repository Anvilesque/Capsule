using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DontDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop Bookshelf");
        if (eventData.pointerDrag != null) 
        {
            eventData.pointerDrag.GetComponent<DragDrop>().transform.position = eventData.pointerDrag.GetComponent<DragDrop>().startingPoint;
        }
    }
}
