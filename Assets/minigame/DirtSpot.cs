using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpot : MonoBehaviour
{
    [SerializeField]
    public GameObject dirt;

    void OnMouseEnter()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log($"this is {dirt.name}");
    }

}
