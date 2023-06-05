using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpot : MonoBehaviour
{
    [SerializeField]
    public GameObject dirt;
    
    public GameObject tool;
    
    public SpriteRenderer dirtSprite;

    void Start()
    {
        dirtSprite = GetComponent<SpriteRenderer>();
    }

    void Update() {
        //
    }

    void OnMouseEnter()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message

        if (!tool.activeSelf) // tool thumbnail selected = inactive image
        {
            if (dirtSprite.color.a > 0)
            {
                dirtSprite.color = new Color(1f,1f,1f,dirtSprite.color.a-.2f);
            }
        } 
    }

}
