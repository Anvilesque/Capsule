using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TaskManager : MonoBehaviour
{
    private Camera mainCam;
    private List<Camera> gameCams;
    private string currentMinigame;
    private PlayerMovementController mvmtControl;
    private TileManager tileManager;
    private Tilemap interactableMap;
    private bool taskAvailable;
    private bool isTasking;
    private string taskName;
    private float isoViewRatio;
    public Canvas canvasBookshelf;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        gameCams = new List<Camera>();
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam.tag == "MainCamera") continue;
            else gameCams.Add(cam);
        }
        mvmtControl = FindObjectOfType<PlayerMovementController>();
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        
        isoViewRatio = 0.2f;
        isTasking = false;
        canvasBookshelf.gameObject.SetActive(false);
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractables();
        if (Input.GetButtonDown("Interact"))
        {
            if (isTasking) {}
            else if (!taskAvailable) {}
            else {
                StartTask(taskName);
            }
        }
    }

    private void CheckInteractables()
    {
        if (mvmtControl.isMoving) return;
        if (isTasking) return;
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
            if (taskAvailable) break;
        }
    }

    public void StartTask(string taskName)
    {
        isTasking = true;
        if (taskName == "Bookshelf")
        {
            currentMinigame = taskName;
            StartBookshelf();
        }
    }

    public void StopTask()
    {
        isTasking = false;
    }

    void StartBookshelf()
    {
        mvmtControl.DisableMovement();
        Camera bookshelfCam = gameCams.Find(x=> x.name.Contains(currentMinigame));
        bookshelfCam.transparencySortMode = TransparencySortMode.Default;
        Debug.Log("Main camera's TransparencySort: " + mainCam.transparencySortMode);
        bookshelfCam.rect = new UnityEngine.Rect(1f, 0, (1 - isoViewRatio), 1f);
        Sequence seqBookshelfStart = DOTween.Sequence();
        seqBookshelfStart.Append(DOTween.To(()=> mainCam.rect, x=> mainCam.rect = x, new UnityEngine.Rect(0, 0, isoViewRatio, 1f), 1));
        seqBookshelfStart.Join(DOTween.To(()=> bookshelfCam.rect, x=> bookshelfCam.rect = x, new UnityEngine.Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f), 1));
        seqBookshelfStart.onComplete = (()=> canvasBookshelf.gameObject.SetActive(true));
    }

    public void StopBookshelf()
    {
        mvmtControl.EnableMovement();
        Tween expandIsometric = DOTween.To(()=> mainCam.rect, x=> mainCam.rect = x, new UnityEngine.Rect(0, 0, 1f, 1f), 1);
        // expandIsometric.onComplete = ()=> canvasBookshelf.gameObject.SetActive(false);
        StopTask();
    }
}
