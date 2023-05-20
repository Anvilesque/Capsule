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
    [SerializeField] public Dictionary<Vector2Int, BSItemInfo> occupiedCells;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
        bookshelfCam.transparencySortMode = TransparencySortMode.Default;
        bookshelfMap = GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
        cellSize = bookshelfMap.cellSize;
        occupiedCells = new Dictionary<Vector2Int, BSItemInfo>();
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

    public bool ItemOnGrid(Vector2 itemPosBottomLeft, Vector2 size)
    {
        return bookshelfMap.cellBounds.Contains(bookshelfMap.WorldToCell(itemPosBottomLeft));
    }

    public bool CheckFit(Vector2 itemPosBottomLeft, List<Vector2Int> cellsFilled)
    {
        foreach (Vector2Int cellRelative in cellsFilled)
        {
            if (!bookshelfMap.cellBounds.Contains((bookshelfMap.WorldToCell(itemPosBottomLeft) + (Vector3Int)cellRelative))) return false;
        }
        return true;
    }

    public bool CheckOccupied(Vector2 itemPosBottomLeft, List<Vector2Int> cellsFilled)
    {
        foreach (Vector2Int cellRelative in cellsFilled)
        {
            if (occupiedCells.ContainsKey(GetCellFromWorldPos(itemPosBottomLeft) + cellRelative)) return true;
        }
        return false;
    }

    public bool CheckStackable(Vector2 itemWorldPos, BSItemInfo heldItemInfo)
    {
        if (!occupiedCells.ContainsKey(GetCellFromWorldPos(itemWorldPos))) return false;
        if (occupiedCells[GetCellFromWorldPos(itemWorldPos)].itemType == heldItemInfo.itemType) return true;
        else return false;
    }

    public Vector2 GetStackPos(Vector2 itemWorldPos)
    {
        int tempID = occupiedCells[GetCellFromWorldPos(itemWorldPos)].itemID;
        bool repeatLoop = true;
        while (repeatLoop)
        {
            repeatLoop = false;
            if (occupiedCells.ContainsKey(GetCellFromWorldPos(itemWorldPos) + Vector2Int.left))
            {
                if (occupiedCells[(GetCellFromWorldPos(itemWorldPos) + Vector2Int.left)].itemID == tempID)
                {
                    itemWorldPos += cellSize * Vector2Int.left;
                    repeatLoop = true;
                }
            }
            if (occupiedCells.ContainsKey(GetCellFromWorldPos(itemWorldPos) + Vector2Int.down))
            {
                if (occupiedCells[(GetCellFromWorldPos(itemWorldPos) + Vector2Int.down)].itemID == tempID)
                {
                    itemWorldPos += cellSize * Vector2Int.down;
                    repeatLoop = true;
                }
            }
        }
        return itemWorldPos;
    }

    public Vector2Int GetCellFromWorldPos(Vector2 pos)
    {
        return (Vector2Int)bookshelfMap.WorldToCell(pos);
    }

    public Vector2 GetWorldFromCellPos(Vector2Int pos)
    {
        return bookshelfMap.CellToWorld((Vector3Int)pos);
    }

    public Vector2 GetWorldCellFromWorldPos(Vector2 pos)
    {
        return bookshelfMap.CellToWorld(bookshelfMap.WorldToCell(pos));
    }

    public void OccupyCells(Vector2Int startingPoint, BSItemInfo itemInfo)
    {
        foreach (Vector2Int cellRelative in itemInfo.cellsFilled)
        {
            occupiedCells.Add(startingPoint + cellRelative, itemInfo);
        }
    }

    public void UnoccupyCells(Vector2Int startingPoint, List<Vector2Int> cellsFilled)
    {
        foreach (Vector2Int cellRelative in cellsFilled)
        {
            
            occupiedCells.Remove(startingPoint + cellRelative);
        }
    }

    public Vector2Int GetClosestCell(Vector2 itemPosBottomLeft)
    {
        Vector2 itemPosBottomLeftRounded = itemPosBottomLeft + 0.5f * cellSize;
        int x = Mathf.Clamp(GetCellFromWorldPos(itemPosBottomLeftRounded).x, bookshelfMap.cellBounds.xMin, bookshelfMap.cellBounds.xMax);
        int y = Mathf.Clamp(GetCellFromWorldPos(itemPosBottomLeftRounded).y, bookshelfMap.cellBounds.yMin, bookshelfMap.cellBounds.yMax);
        return new Vector2Int(x, y);
    }
}
