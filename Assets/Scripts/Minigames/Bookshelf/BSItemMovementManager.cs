using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemMovementManager : MonoBehaviour
{
    private Camera bookshelfCam;
    private BSItemInfo itemInfo;
    private BSGridManager bookshelfGrid;
    private Vector3 itemPosBL;
    private Vector3 itemPosCenter;
    private Vector2 offsetMouseFromTransform;
    private Vector2 offsetSize;
    private Vector2 offsetHeldUp;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        itemInfo = GetComponent<BSItemInfo>();
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfCam = bookshelfGrid.bookshelfCam;
        bookshelfCam.transparencySortMode = TransparencySortMode.Default;

        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);

        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        offsetHeldUp = new Vector2(0f, 0.2f);
        // Size offset: Add half of size * cell size to current position
        offsetSize = new Vector2((itemInfo.size.x / 2.0f) * bookshelfGrid.cellSize.x, (itemInfo.size.y / 2.0f) * bookshelfGrid.cellSize.y);

        // z-value should always be 0, since we're only working with Vector2
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        itemPosCenter = transform.position;
        itemPosCenter.z = 0f;
        UpdateItemPosBLFromCenter();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        bookshelfGrid.UnoccupyCells(bookshelfGrid.GetCellFromWorldPos(itemPosBL), itemInfo.cellsFilled);
    }

    private void OnMouseDrag()
    {
        itemPosCenter = mousePos + offsetMouseFromTransform;
        UpdateItemPosBLFromCenter();
        if (bookshelfGrid.ItemOnGrid(itemPosBL, itemInfo.size))
        {
            if (bookshelfGrid.CheckFit(itemPosBL, itemInfo.cellsFilled))
            {
                itemPosBL = bookshelfGrid.GetWorldCellFromWorldPos(itemPosBL);
                UpdateItemPosCenterFromBL();
            }
        }
        else
        {
            // itemPosCenter += (Vector3)offsetHeldUp;
        }
        transform.position = itemPosCenter;
    }

    private void OnMouseUp()
    {
        if (bookshelfGrid.ItemOnGrid(itemPosBL, itemInfo.size))
        {
            bookshelfGrid.OccupyCells(bookshelfGrid.GetCellFromWorldPos(itemPosBL), itemInfo.cellsFilled);
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
}
