using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSEvaluationManager : MonoBehaviour
{
    BSGridManager bookshelfGrid;
    Tilemap bookshelfMap;
    List<TileBase> tiles;
    int countNumberOfItems;

    // Start is called before the first frame update
    void Start()
    {
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfMap = bookshelfGrid.GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
        tiles = new List<TileBase>(bookshelfMap.GetTilesBlock(bookshelfMap.cellBounds));
    }

    public void EvaluateNumberOfItems()
    {
        countNumberOfItems = 0;
        foreach (Vector3Int tile in bookshelfMap.cellBounds.allPositionsWithin)
        {
            if (bookshelfGrid.occupiedCells.ContainsKey((Vector2Int)tile))
            {
                countNumberOfItems++;
            }
        }
        Debug.Log(countNumberOfItems);
    }
}
