using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSObjectMovementManager : MonoBehaviour
{
    Camera cam;
    private BSGridManager bookshelfGrid;
    private Vector3 currentPos;
    private Vector3 offsetMouseFromTransform;
    private Vector3 offsetMouseFromNearestCell;
    private Vector3 offsetHeldUp;
    private Vector3 mousePos;
    private bool isHeld;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        cam.transparencySortMode = TransparencySortMode.Default;
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        currentPos = transform.position;
        offsetMouseFromTransform = transform.position - mousePos;
        offsetHeldUp = new Vector3(0f, 0.2f, 0f);
        isHeld = false;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        // nearestCellPos = bookshelfGrid.WorldToWorldPosOnGrid(mousePos);
    }

    private void OnMouseDown() {
        isHeld = true;
        offsetMouseFromTransform = transform.position - mousePos;
    }

    private void OnMouseDrag()
    {
        if (isHeld)
        {
            if (bookshelfGrid.MouseOnGrid())
            {
                // Do grid behavior
                currentPos = mousePos;
            }
            else
            {
                // Do outside behavior
                currentPos = mousePos;
            }
            currentPos += offsetHeldUp;
            transform.position = currentPos;
        }
    }

    private void OnMouseUp()
    {
        isHeld = false;
        if (bookshelfGrid.MouseOnGrid())
        {

        }
        else
        {
            currentPos -= offsetHeldUp;
            transform.position = currentPos;
        }
    }
}
