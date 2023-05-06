using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemMovementManager : MonoBehaviour
{
    private BSItemInfo itemInfo;
    private Camera bookshelfCam;
    private BSGridManager bookshelfGrid;
    private SpriteRenderer sprite;
    private Vector2 mousePos;
    private Vector3 prevPosBL;
    private Vector3 itemPosBL;
    private Vector3 itemPosCenter;
    private Vector2 offsetMouseFromTransform;
    private Vector2 offsetSize;
    private Vector2 offsetHeldUp;
    private float snapTolerance;
    private float preventTransparency = 0.5f;
    private bool canPlace;
    

    // Start is called before the first frame update
    void Start()
    {
        itemInfo = GetComponent<BSItemInfo>();
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfCam = bookshelfGrid.bookshelfCam;
        sprite = GetComponent<SpriteRenderer>();

        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);

        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        offsetHeldUp = new Vector2(0f, 0.2f);
        // Size offset: Add half of size * cell size to current position
        offsetSize = new Vector2((itemInfo.size.x / 2.0f) * bookshelfGrid.cellSize.x, (itemInfo.size.y / 2.0f) * bookshelfGrid.cellSize.y);
        snapTolerance = 0.7f * bookshelfGrid.cellSize.x;

        // z-value should always be 0, since we're only working with Vector2
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        itemPosCenter = transform.position;
        itemPosCenter.z = 0f;
        UpdateItemPosBLFromCenter();
        prevPosBL = itemPosBL;
        canPlace = true;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        sprite.sortingOrder = 1;
        prevPosBL = itemPosBL;
        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        bookshelfGrid.UnoccupyCells(bookshelfGrid.GetCellFromWorldPos(itemPosBL), itemInfo.cellsFilled);
    }

    private void OnMouseDrag()
    {
        UnPreventPlacement();

        itemPosCenter = mousePos + offsetMouseFromTransform;
        UpdateItemPosBLFromCenter();

        if (bookshelfGrid.ItemOnGrid(itemPosBL, itemInfo.size))
        {
            if (!bookshelfGrid.CheckFit(itemPosBL, itemInfo.cellsFilled) || bookshelfGrid.CheckOccupied(itemPosBL, itemInfo.cellsFilled))
            {
                PreventPlacement();
            }
            else
            {
                itemPosBL = bookshelfGrid.GetWorldCellFromWorldPos(itemPosBL);
                UpdateItemPosCenterFromBL();
            }
        }
        else
        {
            // Check snap tolerance
            Vector2Int closestCell = bookshelfGrid.GetClosestCell(itemPosBL);
            bool canSnap = Vector2.Distance(itemPosBL, bookshelfGrid.GetWorldFromCellPos(closestCell)) <= snapTolerance;
            if (canSnap)
            {
                Vector2 closestCellPos = bookshelfGrid.GetWorldFromCellPos(closestCell);
                if (!bookshelfGrid.CheckFit(closestCellPos, itemInfo.cellsFilled) || bookshelfGrid.CheckOccupied(closestCellPos, itemInfo.cellsFilled))
                {
                    PreventPlacement();
                }
                else
                {
                    itemPosBL = closestCellPos;
                    UpdateItemPosCenterFromBL();
                }
            }
            // itemPosCenter += (Vector3)offsetHeldUp;
        }
        transform.position = itemPosCenter;
    }

    private void OnMouseUp()
    {
        sprite.sortingOrder = 0;
        if (!canPlace)
        {
            itemPosBL = prevPosBL;
            UpdateItemPosCenterFromBL();
            transform.position = itemPosCenter;
            UnPreventPlacement();
        }
        else if (bookshelfGrid.ItemOnGrid(itemPosBL, itemInfo.size))
        {
            bookshelfGrid.OccupyCells(bookshelfGrid.GetCellFromWorldPos(itemPosBL), itemInfo);
        }
        else
        {
            // itemPosCenter -= (Vector3)offsetHeldUp;
            transform.position = itemPosCenter;
        }
    }

    private void UpdateItemPosCenterFromBL()
    {
        itemPosCenter = itemPosBL + (Vector3)offsetSize;   
    }

    private void UpdateItemPosBLFromCenter()
    {
        itemPosBL = itemPosCenter - (Vector3)offsetSize;
    }
    
    private void PreventPlacement()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, preventTransparency);
        canPlace = false;
    }

    private void UnPreventPlacement()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        canPlace = true;
    }
}
