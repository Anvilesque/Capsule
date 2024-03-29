using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class antiqueManager : MonoBehaviour
{
    [SerializeField]
    public GameObject antique;
    public List<GameObject> dirtSpots; 
    bool complete;

    // Start is called before the first frame update
    void Start()
    {
        complete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!complete)
        {
            complete = true;
            foreach (GameObject dirt in dirtSpots)
            {
                if (dirt.GetComponent<SpriteRenderer>().color.a > 0)
                {
                    complete = false;
                    // Debug.Log("opacity of " + dirt.name + ": " + dirt.GetComponent<SpriteRenderer>().color.a);
                }
            }

            if (complete) 
            {
                // Debug.Log("clean!!");  
                MoveOffScreen();
            }
        }
    }

    void MoveOffScreen()
    {
        // animate movement
        gameObject.SetActive(false);
        complete = false;
        foreach (GameObject dirt in dirtSpots)
        {
            dirt.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        }
    }
}
