using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSGridManager : MonoBehaviour
{
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 12;
    public Camera bookshelfCam;
    private Tilemap bookshelfMap;

    // public Vector3 cellSize {get; private set;}
    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
        bookshelfMap = GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = bookshelfCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
    }

    public bool MouseOnGrid()
    {
        return bookshelfMap.cellBounds.Contains(bookshelfMap.WorldToCell(mousePos));
    }
}
