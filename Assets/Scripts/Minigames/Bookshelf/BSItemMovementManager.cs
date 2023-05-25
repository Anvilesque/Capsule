using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemMovementManager : MonoBehaviour
{
    private BSItemInfo itemInfo;
    private Camera bookshelfCam;
    private BSGridManager bookshelfGrid;
    private Vector2 mousePos;
    private Vector3 prevPosBottomLeft;
    private Vector3 itemPosBottomLeft;
    private Vector3 itemPosCenter;
    private Vector2 offsetMouseFromTransform;
    private Vector2 offsetSize;
    private Vector2 offsetHeldUp;
    private Vector2 offsetStacked;
    private float snapTolerance;
    private float preventTransparency = 0.5f;
    private bool canPlace;    

    // Start is called before the first frame update
    void Start()
    {
        itemInfo = GetComponent<BSItemInfo>();
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfCam = bookshelfGrid.bookshelfCam;

        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);

        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        offsetHeldUp = new Vector2(0f, 0.2f);
        // Size offset: Add half of size * cell size to current position
        offsetSize = new Vector2((itemInfo.itemSize.x / 2.0f) * bookshelfGrid.cellSize.x, (itemInfo.itemSize.y / 2.0f) * bookshelfGrid.cellSize.y);
        offsetStacked = new Vector2(0, 0.2f);
        snapTolerance = 0.7f * bookshelfGrid.cellSize.x;

        // z-value should always be 0, since we're only working with Vector2
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        itemPosCenter = transform.position;
        UpdateItemPosBLFromCenter();
        prevPosBottomLeft = itemPosBottomLeft;
        UnPreventPlacement();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        itemInfo.sprite.sortingOrder = 10;
        prevPosBottomLeft = itemPosBottomLeft;
        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        Vector2Int targetCell = bookshelfGrid.GetCellFromWorldPos(itemPosBottomLeft);
        if (!bookshelfGrid.occupiedCells.ContainsKey(targetCell)) return;
        if (bookshelfGrid.occupiedCells[targetCell].isStacked)
        {
            bookshelfGrid.UnStackItem(itemPosBottomLeft, itemInfo);
        }
        else bookshelfGrid.UnoccupyCells(targetCell, itemInfo);
    }

    private void OnMouseDrag()
    {
        UnPreventPlacement();

        itemPosCenter = mousePos + offsetMouseFromTransform;
        if (!bookshelfGrid.snapPreviewEnabled) transform.position = itemPosCenter;
        UpdateItemPosBLFromCenter();

        CalculateItemCenter();
        if (bookshelfGrid.snapPreviewEnabled) transform.position = itemPosCenter;
        
    }

    // private void OnDrawGizmos() {
    //     if (!Application.isPlaying) return;
    //     Gizmos.DrawCube(bookshelfGrid.GetWorldFromCellPos(bookshelfGrid.GetClosestCell(itemPosBottomLeft)), new Vector3(0.5f, 0.5f, 0.5f));
    // }

    private void OnMouseUp()
    {
        itemInfo.sprite.sortingOrder = 0;

        if (!canPlace)
        {
            ReturnItemToPreviousPosition();
            return;
        }

        itemInfo.isBookshelfed = true;
        transform.position = itemPosCenter;
        Vector2Int targetCell = bookshelfGrid.GetCellFromWorldPos(itemPosBottomLeft);
        if (bookshelfGrid.CheckOccupied(itemPosBottomLeft, itemInfo.cellsFilled))
        {
            bookshelfGrid.StackItem(itemPosBottomLeft, itemInfo);
            transform.position += (itemInfo.stackCount - 1) * (Vector3)offsetStacked;
        }
        else bookshelfGrid.OccupyCells(targetCell, itemInfo);
    }

    private void CalculateItemCenter()
    {
        if (bookshelfGrid.ItemOnGrid(itemPosBottomLeft, itemInfo.itemSize)) 
        {
            AttemptPlacement(bookshelfGrid.GetWorldCellFromWorldPos(itemPosBottomLeft));
        }
        else
        {
            AttemptSnap();
        }
    }

    void AttemptPlacement(Vector2 itemWorldPos)
    {
        if (!bookshelfGrid.CheckSupport(itemWorldPos, itemInfo.cellsFilled))
        {
            PreventPlacement();
            return;
        }
        if (!bookshelfGrid.CheckFit(itemWorldPos, itemInfo.cellsFilled))
        {
            PreventPlacement();
            return;
        }
        if (bookshelfGrid.CheckOccupied(itemWorldPos, itemInfo.cellsFilled))
        {
            if (!itemInfo.isStackable)
            {
                PreventPlacement();
                return;
            }

            bool stackSuccess = false;
            foreach (Vector2Int cellRelative in itemInfo.cellsFilled)
            {
                Vector2 tempWorldPos = itemWorldPos + bookshelfGrid.cellSize * cellRelative;
                if (bookshelfGrid.CheckStackable(tempWorldPos, itemInfo))
                {
                    itemWorldPos = bookshelfGrid.GetStackPos(tempWorldPos);
                    stackSuccess = true;
                    break;
                }
            }
            if (!stackSuccess)
            {
                PreventPlacement();
                return;
            }
        }
        itemPosBottomLeft = itemWorldPos;
        UpdateItemPosCenterFromBL();
    }

    void AttemptSnap()
    {
        Vector2Int closestCell = bookshelfGrid.GetClosestCell(itemPosBottomLeft);
        bool canSnap = Vector2.Distance(itemPosBottomLeft, bookshelfGrid.GetWorldFromCellPos(closestCell)) <= snapTolerance;
        if (!canSnap)
        {
            PreventPlacement();
            return;
        }

        Vector2 closestCellPos = bookshelfGrid.GetWorldFromCellPos(closestCell);
        AttemptPlacement(closestCellPos);
    }

    private void UpdateItemPosCenterFromBL()
    {
        itemPosCenter = itemPosBottomLeft + (Vector3)offsetSize;   
    }

    private void UpdateItemPosBLFromCenter()
    {
        itemPosBottomLeft = itemPosCenter - (Vector3)offsetSize;
    }
    
    private void PreventPlacement()
    {
        itemInfo.sprite.color = new Color(itemInfo.sprite.color.r, itemInfo.sprite.color.g, itemInfo.sprite.color.b, preventTransparency);
        canPlace = false;
    }

    private void UnPreventPlacement()
    {
        itemInfo.sprite.color = new Color(itemInfo.sprite.color.r, itemInfo.sprite.color.g, itemInfo.sprite.color.b, 1f);
        canPlace = true;
    }

    private void ReturnItemToPreviousPosition()
    {
        itemPosBottomLeft = prevPosBottomLeft;
        UpdateItemPosCenterFromBL();
        transform.position = itemPosCenter;
        Vector2Int targetCell = bookshelfGrid.GetCellFromWorldPos(itemPosBottomLeft);
        if (bookshelfGrid.CheckOccupied(itemPosBottomLeft, itemInfo.cellsFilled))
        {
            bookshelfGrid.StackItem(itemPosBottomLeft, itemInfo);
            transform.position += (itemInfo.stackCount - 1) * (Vector3)offsetStacked;
        }
        else bookshelfGrid.OccupyCells(targetCell, itemInfo);
        UnPreventPlacement();
        
    }
}
