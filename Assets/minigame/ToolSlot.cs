using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolSlot : MonoBehaviour
{
    private GameObject toolSpriteObject;
    public Texture2D toolCursor;
    public bool toolSelected {get; private set;}

    private List<ToolSlot> otherTools;
    
    private void Start()
    {
        toolSpriteObject = transform.GetChild(0).gameObject;
        toolSelected = false;
        otherTools = new List<ToolSlot>(FindObjectsOfType<ToolSlot>());
        otherTools.Remove(this);
    }

    public void SelectTool()
    {
        if (toolSelected) return;
        toolSpriteObject.SetActive(false);
        toolSelected = true;
        if (toolCursor == null) return;
        Cursor.SetCursor(toolCursor, Vector2.zero, CursorMode.Auto);
    }

    public void DeselectTool()
    {
        if (!toolSelected) return;
        toolSpriteObject.SetActive(true);
        toolSelected = false;
        if (toolCursor == null) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void ToggleSelect()
    {
        foreach (ToolSlot tool in otherTools)
        {
            if (tool.toolSelected) tool.DeselectTool();
        }
        if (toolSelected) DeselectTool();
        else SelectTool();
    }
}
