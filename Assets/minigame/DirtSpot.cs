using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpot : MonoBehaviour
{
    [SerializeField]
    public GameObject dirt;
    
    public GameObject tool;
    public SpriteRenderer dirtSprite;
    float opacity;

    void Start()
    {
        opacity = 1;
        dirtSprite = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (opacity <= 0)
        {
            // disable object
            dirt.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log($"this is {dirt.name}");

        if (!tool.activeSelf) // tool thumbnail selected = inactive image
        {
            Debug.Log($"you have selected {tool.name}");
            if (opacity > 0)
            {
                opacity -= .2f;
            }
        } else if (tool.activeSelf) // tool thumbnail unselected = active image
            Debug.Log($"you have not selected {tool.name}");

        dirtSprite.color = new Color(1f,1f,1f,opacity);
    }

}
