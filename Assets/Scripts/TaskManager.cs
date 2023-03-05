using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TaskManager : MonoBehaviour
{
    private Camera mainCam;
    private PlayerMovementController mvmtControl;
    public Canvas canvasBookshelf;
    private TileManager tileManager;
    private Tilemap interactableMap;
    private bool taskAvailable;
    private string taskName;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        mvmtControl = FindObjectOfType<PlayerMovementController>();
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        canvasBookshelf.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractables();
        if (Input.GetButtonDown("Interact"))
        {
            if (!taskAvailable) {}
            else {
                StartTask(taskName);
            }
        }
    }

    private void CheckInteractables()
    {
        if (mvmtControl.isMoving) return;
        Vector3Int tileUp = TileManager.WorldCoordsToGridCoords(mvmtControl.currentPosWorld) + new Vector3Int(0, -1, 0);
        Vector3Int tileDown = TileManager.WorldCoordsToGridCoords(mvmtControl.currentPosWorld) + new Vector3Int(0, +1, 0);
        Vector3Int tileLeft = TileManager.WorldCoordsToGridCoords(mvmtControl.currentPosWorld) + new Vector3Int(-1, 0, 0);
        Vector3Int tileRight = TileManager.WorldCoordsToGridCoords(mvmtControl.currentPosWorld) + new Vector3Int(+1, 0, 0);
        List<Vector3Int> adjacentTiles = new List<Vector3Int>() {tileUp, tileDown, tileLeft, tileRight};
        foreach (Vector3Int tile in adjacentTiles)
        {
            Vector3Int tempTile = tile;
            taskAvailable = false;
            for (int i = interactableMap.cellBounds.zMin; i <= interactableMap.cellBounds.zMax; i++)
            {
                tempTile.z = i;
                if (interactableMap.HasTile(tempTile))
                {
                    taskName = tileManager.GetTileData(interactableMap, tempTile).taskName;
                    taskAvailable = true;
                    break;
                }
            }
        }
    }

    public void StartTask(string taskName)
    {
        if (taskName == "Bookshelf")
        {
            StartBookshelf();
        }
    }

    void StartBookshelf()
    {
        mvmtControl.DisableMovement();
        mainCam.rect = new UnityEngine.Rect(0, 0, 0.2f, 1f);
        canvasBookshelf.gameObject.SetActive(true);
    }

    void StopBookshelf()
    {
        mvmtControl.EnableMovement();
        mainCam.rect = new UnityEngine.Rect(0, 0, 0f, 1f);
        canvasBookshelf.enabled = false;
    }
}
