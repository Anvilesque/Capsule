using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSEvaluationManager : MonoBehaviour
{
    BSGridManager bookshelfGrid;
    Tilemap bookshelfMap;
    List<BSItemInfo> items;
    private int countNumberOfItems;
    private int countTotalItems;

    // Start is called before the first frame update
    void Start()
    {
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfMap = bookshelfGrid.GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
        items = new List<BSItemInfo>(FindObjectsOfType<BSItemInfo>());
    }

    public void EvaluateNumberOfItems()
    {
        countNumberOfItems = 0;
        countTotalItems = 0;
        foreach (BSItemInfo item in items)
        {
            countTotalItems++;
            if (item.isBookshelfed)
            {
                countNumberOfItems++;
            }
        }
    }

    public void EvaluateNumberOfTypes()
    {

    }

    public void EvaluateType()
    {

    }

    public void EvaluateColor()
    {

    }

    public void EvaluateSubsize()
    {

    }

    public void EvaluateSymmetry()
    {

    }

    public void EvaluateStacked()
    {

    }
}
