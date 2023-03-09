using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (transform.childCount >= 4){
            Debug.Log("More than 4 items in this slot.");
            return;
        }
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag = transform;
        Debug.Log("slot assigned parentAfterDrag");
    }
}
