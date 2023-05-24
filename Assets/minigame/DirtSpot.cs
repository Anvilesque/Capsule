using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpot : MonoBehaviour
{
    [SerializeField]
    public GameObject dirt;
    [SerializeField]
    public GameObject tool;

    void OnMouseEnter()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log($"this is {dirt.name}");

        if (!tool.activeSelf) // tool thumbnail selected = inactive image
            Debug.Log($"you have selected {tool.name}");
        if (tool.activeSelf) // tool thumbnail unselected = active image
            Debug.Log($"you have not selected {tool.name}");
    }   

}
