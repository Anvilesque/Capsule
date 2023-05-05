using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSObjectMovementManager : MonoBehaviour
{
    Camera cam;
    private BSObjectInfo objectInfo;
    private BSGridManager bookshelfGrid;
    private Vector3 currentPos;
    private Vector2 offsetMouseFromTransform;
    private Vector2 offsetMouseFromNearestCell;
    private Vector2 offsetHeldUp;
    private Vector2 mousePos;
    private bool isHeld;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        cam.transparencySortMode = TransparencySortMode.Default;
        objectInfo = GetComponent<BSObjectInfo>();
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        // z-value should always be 0, since we're only working with Vector2
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        currentPos = transform.position;
        currentPos.z = 0f;
        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
        offsetHeldUp = new Vector2(0f, 0.2f);
        isHeld = false;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        // nearestCellPos = bookshelfGrid.WorldToWorldPosOnGrid(mousePos);
    }

    private void OnMouseDown() {
        isHeld = true;
        offsetMouseFromTransform = transform.position - (Vector3)mousePos;
    }

    private void OnMouseDrag()
    {
        if (isHeld)
        {
            bookshelfGrid.UnoccupyCells(bookshelfGrid.GetCellFromWorldPos(currentPos), objectInfo.cellsFilled);
            if (bookshelfGrid.MouseOnGrid())
            {
                // Do grid behavior
                currentPos = bookshelfGrid.GetWorldCellFromWorldPos(mousePos);
                // Size offset: Add half of size * cell size to current position
                Vector2 sizeOffset = new Vector2((objectInfo.size.x / 2.0f) * bookshelfGrid.cellSize.x, (objectInfo.size.y / 2.0f) * bookshelfGrid.cellSize.y);
                currentPos += (Vector3)sizeOffset;
            }
            else
            {
                // Do outside behavior
                currentPos = mousePos + offsetMouseFromTransform;
            }
            currentPos += (Vector3)offsetHeldUp;
            transform.position = currentPos;
        }
    }

    private void OnMouseUp()
    {
        isHeld = false;
        if (bookshelfGrid.MouseOnGrid())
        {
            currentPos -= (Vector3)offsetHeldUp;
            transform.position = currentPos;
            bookshelfGrid.OccupyCells(bookshelfGrid.GetCellFromWorldPos(currentPos), objectInfo.cellsFilled);
        }
        else
        {
            currentPos -= (Vector3)offsetHeldUp;
            transform.position = currentPos;
        }
    }
}
