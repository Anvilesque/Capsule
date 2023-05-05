using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSGridManager : MonoBehaviour
{
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 12;
    public Vector2 cellSize {get; private set;}
    public Camera bookshelfCam;
    private Tilemap bookshelfMap;
    public List<Vector2Int> occupiedCells;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
        bookshelfMap = GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
        cellSize = bookshelfMap.cellSize;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
    }

    public bool MouseOnGrid()
    {
        return bookshelfMap.cellBounds.Contains(bookshelfMap.WorldToCell(mousePos));
    }

    public bool ItemOnGrid(Vector2 itemPosBL, Vector2 size)
    {
        return bookshelfMap.cellBounds.Contains(bookshelfMap.WorldToCell(itemPosBL));
    }

    public bool CheckFit(Vector2 itemPosBL, List<Vector2Int> cellsFilled)
    {
        foreach (Vector2Int cellRelative in cellsFilled)
        {
            if (!bookshelfMap.cellBounds.Contains((bookshelfMap.WorldToCell(itemPosBL) + (Vector3Int)cellRelative))) return false;
        }
        return true;
    }

    public Vector2Int GetCellFromWorldPos(Vector2 pos)
    {
        return (Vector2Int)bookshelfMap.WorldToCell(pos);
    }

    public Vector2 GetWorldCellFromWorldPos(Vector2 pos)
    {
        return bookshelfMap.CellToWorld(bookshelfMap.WorldToCell(pos));
    }

    public void OccupyCells(Vector2Int startingPoint, List<Vector2Int> cellsFilled)
    {
        foreach (Vector2Int cellRelative in cellsFilled)
        {
            occupiedCells.Add(startingPoint + cellRelative);
        }
    }

    public void UnoccupyCells(Vector2Int startingPoint, List<Vector2Int> cellsFilled)
    {
        foreach (Vector2Int cellRelative in cellsFilled)
        {
            
            occupiedCells.Remove(startingPoint + cellRelative);
        }
    }
}
