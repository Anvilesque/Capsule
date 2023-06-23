using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiqueObject : MonoBehaviour
{
    private List<SpriteRenderer> dirtSpots;
    private bool isClean;

    // Start is called before the first frame update
    void Start()
    {
        dirtSpots = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));
        dirtSpots.Remove(GetComponent<SpriteRenderer>());
        isClean = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClean) return;
        CheckClean();
        if (isClean) 
        { 
            ResetAntique();
        }
    }

    void CheckClean()
    {
        isClean = true;
        foreach (SpriteRenderer dirt in dirtSpots)
        {
            if (dirt.color.a > 0)
            {
                isClean = false;
            }
        }
    }

    public void ResetAntique()
    {
        // animate movement
        gameObject.SetActive(false);
        isClean = false;
        foreach (SpriteRenderer dirt in dirtSpots)
        {
            dirt.color = new Color(1f,1f,1f,1f);
        }
    }
}
