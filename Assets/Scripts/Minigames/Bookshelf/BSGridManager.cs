using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSGridManager : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    public Vector2 mousePos;
    private const int DEFAULT_LAYER = 0;
    private const int IGNORE_RAYCAST_LAYER = 2;
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 12;
    public Vector2 cellSize {get; private set;}
    public Camera bookshelfCam;
    private Tilemap bookshelfMap;
    public Dictionary<Vector2Int, BSItemInfo> occupiedCells;
    private List<int> shelfPositions;
    private List<GameObject> shelfObjects;
    public SpriteRenderer bookshelfSprite;
    List<int> possibleIntervals = new List<int>() {6, 4, 3, 2};
    public int shelfInterval {get; private set;}
    public bool snapPreviewEnabled {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        bookshelfMap = GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
        cellSize = bookshelfMap.cellSize;
        occupiedCells = new Dictionary<Vector2Int, BSItemInfo>();
        snapPreviewEnabled = saveData.snapPreviewEnabled;
        shelfPositions = new List<int>();
        shelfObjects = new List<GameObject>();
        shelfInterval = saveData.shelfInterval;
        UpdateShelfPositions();
    }

    // Update is called once per frame
    void Update()
    {
        if (bookshelfCam.rect.x == 1) return;
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void ToggleSnapPreview()
    {
        snapPreviewEnabled = !snapPreviewEnabled;
    }

    public void UpdateShelfPositions()
    {
        foreach (GameObject shelf in shelfObjects)
        {
            Destroy(shelf);
        }
        shelfObjects.Clear();
        shelfPositions.Clear();

        int tempShelfPosition = bookshelfMap.cellBounds.yMin;
        while (tempShelfPosition <= bookshelfMap.cellBounds.yMax)
        {
            shelfPositions.Add(tempShelfPosition);
            if (tempShelfPosition != bookshelfMap.cellBounds.yMin && tempShelfPosition != bookshelfMap.cellBounds.yMax)
            {
                GameObject newShelf = (GameObject)Instantiate(Resources.Load("Prefabs/Minigames/Bookshelf/Bookshelf_Shelf"), Vector3.zero, Quaternion.identity, bookshelfSprite.transform);
                shelfObjects.Add(newShelf);
                newShelf.transform.position = bookshelfMap.CellToWorld(new Vector3Int(0, tempShelfPosition, 0));
            }
            tempShelfPosition += shelfInterval;
        }
    }

    public void ChangeShelfInterval()
    {
        int nextIndex = (possibleIntervals.FindIndex(interval => interval == shelfInterval) + 1) % possibleIntervals.Count;
        shelfInterval = possibleIntervals[nextIndex];
        UpdateShelfPositions();
    }

    public bool MouseOnGrid()
    {
        return bookshelfMap.cellBounds.Contains(bookshelfMap.WorldToCell(mousePos));
    }

    public bool ItemOnGrid(Vector2 itemPosBottomLeft, Vector2 size)
    {
        return bookshelfMap.cellBounds.Contains(bookshelfMap.WorldToCell(itemPosBottomLeft));
    }

    public bool CheckSupport(Vector2 itemPosBottomLeft, List<Vector2Int> cellsFilledRelative)
    {
        foreach (Vector2Int cellRelative in cellsFilledRelative)
        {
            Vector2Int currentCell = GetCellFromWorldPos(itemPosBottomLeft);
            if (cellRelative.y != 0) continue;
            if (shelfPositions.Contains(currentCell.y)) return true;
            Vector2Int cellBelow = currentCell + cellRelative + Vector2Int.down;
            if (!occupiedCells.ContainsKey(cellBelow)) return false;
            if (!occupiedCells[cellBelow].canSupport) return false;
        }
        return true;
    }

    public bool CheckFit(Vector2 itemPosBottomLeft, List<Vector2Int> cellsFilledRelative)
    {
        int currentShelf = bookshelfMap.cellBounds.yMin;
        foreach (int shelfPosition in shelfPositions)
        {
            if (shelfPosition <= bookshelfMap.WorldToCell(itemPosBottomLeft).y && shelfPosition >= currentShelf)
                currentShelf = shelfPosition;
        }
        int nextShelfIndex = Mathf.Min((shelfPositions.FindIndex(position => position == currentShelf) + 1), shelfPositions.Count - 1);
        int nextShelf = shelfPositions[nextShelfIndex];
        foreach (Vector2Int cellRelative in cellsFilledRelative)
        {
            if (!bookshelfMap.cellBounds.Contains((bookshelfMap.WorldToCell(itemPosBottomLeft) + (Vector3Int)cellRelative))) return false;
            if ((bookshelfMap.WorldToCell(itemPosBottomLeft) + (Vector3Int)cellRelative).y == nextShelf) return false;
        }
        
        return true;
    }

    public bool CheckOccupied(Vector2 itemPosBottomLeft, List<Vector2Int> cellsFilledRelative)
    {
        foreach (Vector2Int cellRelative in cellsFilledRelative)
        {
            if (occupiedCells.ContainsKey(GetCellFromWorldPos(itemPosBottomLeft) + cellRelative)) return true;
        }
        return false;
    }

    public bool CheckStackable(Vector2 itemWorldPos, BSItemInfo heldItemInfo)
    {
        if (!occupiedCells.ContainsKey(GetCellFromWorldPos(itemWorldPos))) return false;
        BSItemInfo itemOnShelf = occupiedCells[GetCellFromWorldPos(itemWorldPos)];
        if (!itemOnShelf.isStackable) return false;
        if (itemOnShelf.itemType == heldItemInfo.itemType && itemOnShelf.itemSize == heldItemInfo.itemSize) return true;
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
        itemInfo.isBookshelfed = true;
        foreach (Vector2Int cellRelative in itemInfo.cellsFilledRelative)
        {
            occupiedCells.Add(startingPoint + cellRelative, itemInfo);
            itemInfo.cellsOccupied.Add(startingPoint + cellRelative);
        }
    }

    public void UnoccupyCells(Vector2Int startingPoint, BSItemInfo itemInfo)
    {
        itemInfo.isBookshelfed = false;
        foreach (Vector2Int cellRelative in itemInfo.cellsFilledRelative)
        {
            occupiedCells.Remove(startingPoint + cellRelative);
            itemInfo.cellsOccupied.Clear();
        }
    }

    public void StackItem(Vector2 itemWorldPos, BSItemInfo itemInfo)
    {
        BSItemInfo itemInGrid = occupiedCells[GetCellFromWorldPos(itemWorldPos)];
        itemInGrid.stackedItems.Peek().gameObject.layer = IGNORE_RAYCAST_LAYER;
        itemInGrid.stackedItems.Push(itemInfo);
        itemInGrid.stackCount += 1;
        itemInfo.gameObject.layer = DEFAULT_LAYER;
        foreach (BSItemInfo item in itemInGrid.stackedItems) item.stackCount = itemInGrid.stackCount;
        itemInGrid.isStacked = true;
        itemInfo.isStacked = true;
        itemInfo.cellsOccupied = itemInGrid.cellsOccupied;
        itemInfo.sprite.sortingOrder = -itemInGrid.stackCount;
    }

    public void UnStackItem(Vector2 itemWorldPos, BSItemInfo itemInfo)
    {
        BSItemInfo itemInGrid = occupiedCells[GetCellFromWorldPos(itemWorldPos)];
        itemInGrid.stackedItems.Pop();
        itemInGrid.stackCount -= 1;
        foreach (BSItemInfo item in itemInGrid.stackedItems) item.stackCount = itemInGrid.stackCount;
        itemInfo.stackCount = 1;
        itemInGrid.stackedItems.Peek().gameObject.layer = DEFAULT_LAYER;
        if (itemInGrid.stackCount == 1) itemInGrid.isStacked = false;
        itemInfo.isStacked = false;
        itemInfo.cellsOccupied.Clear();
    }

    public Vector2Int GetClosestCell(Vector2 itemPosBottomLeft)
    {
        Vector2 itemPosBottomLeftRounded = itemPosBottomLeft + 0.5f * cellSize;
        int x = Mathf.Clamp(GetCellFromWorldPos(itemPosBottomLeftRounded).x, bookshelfMap.cellBounds.xMin, bookshelfMap.cellBounds.xMax);
        int y = Mathf.Clamp(GetCellFromWorldPos(itemPosBottomLeftRounded).y, bookshelfMap.cellBounds.yMin, bookshelfMap.cellBounds.yMax);
        return new Vector2Int(x, y);
    }

    public void ClearItems(float percentageChance)
    {
        List<GameObject> itemsToDestroy = new List<GameObject>();
        foreach (BSItemInfo item in FindObjectsOfType<BSItemInfo>())
        {
            if (!item.isBookshelfed) continue;
            if (Random.Range(0, 100) >= percentageChance) continue;
            
            if (item.isStacked)
            {
                itemsToDestroy.Add(item.stackedItems.Peek().gameObject);
                UnStackItem(bookshelfMap.CellToWorld((Vector3Int)item.cellsOccupied[0]), item.stackedItems.Peek());
            }
            else
            {
                itemsToDestroy.Add(item.gameObject);
                UnoccupyCells(item.cellsOccupied[0], item);
            }
        }
        foreach (GameObject obj in itemsToDestroy)
        {
            Destroy(obj);
        }
    }
}
