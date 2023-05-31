using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class antiqueManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> dirtSpots; 
    bool complete;

    // Start is called before the first frame update
    void Start()
    {
        complete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (complete) {}
        else
        {
            complete = true;
            foreach (GameObject dirt in dirtSpots)
            {
                if (dirt.activeSelf)
                    complete = false;
            }

            if (complete)
                Debug.Log("clean!!");  
                // MoveOffScreen;
        }

    }

    // void MoveOffScreen()
    // {
       
    // }
}
