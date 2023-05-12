using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class toolSlot : MonoBehaviour
{
    [SerializeField] 
    public GameObject item;
    public bool item_selected = false;

    [SerializeField] private Texture2D tool_cursor;

    public void SelectItem()
    {
        Debug.Log("select this slot");
        item.SetActive(item_selected);
        item_selected = !(item_selected);

        if(item_selected)
            Cursor.SetCursor(tool_cursor, Vector2.zero, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);




    }

    public void DeselectItem()
    {
        Debug.Log("deselect the other slots");
        item.SetActive(true);
        item_selected = false;
    }
}
