using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TaskManager : MonoBehaviour
{
    private Camera mainCam;
    private List<Camera> gameCams;
    private PlayerMovementController mvmtControl;
    private Sprite playerSprite;
    private TileManager tileManager;
    private Tilemap interactableMap;
    public GameObject interactIndicator;

    private Camera bookshelfCam;
    private Camera diaryCam;

    private string currentMinigame;
    private bool taskAvailable;
    private bool isTasking;
    private string taskName;
    private float isoViewRatio;
    public Canvas canvasBookshelf;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        gameCams = new List<Camera>();
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam == Camera.main) continue;
            else
            {
                gameCams.Add(cam);
                cam.transparencySortMode = TransparencySortMode.Default;
            }
        }
        mvmtControl = FindObjectOfType<PlayerMovementController>();
        playerSprite = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().sprite;
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        
        bookshelfCam = gameCams.Find(x=> x.name.Contains("Bookshelf"));
        bookshelfCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        diaryCam = gameCams.Find(x=> x.name.Contains("Diary"));
        diaryCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        isoViewRatio = 0.2f;
        isTasking = false;
        // canvasBookshelf.gameObject.SetActive(false);
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractables();
        IndicateInteract();
        if (Input.GetButtonDown("Interact"))
        {
            
            if (!taskAvailable) {}
            else if (isTasking)
            {
                StopTask();
            }
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

    void IndicateInteract()
    {
        if (isTasking)
        {
            DestroyIndicator();
        }
        else if (taskAvailable)
        {
            if (interactIndicator != null) {}
            else
            {
                interactIndicator = Instantiate((GameObject)Resources.Load("Prefabs/InteractIndicator"), mvmtControl.transform.position, Quaternion.identity, mvmtControl.transform);
                interactIndicator.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                interactIndicator.transform.localPosition = new Vector3(-0.5f, 1f, 100f);
            }
        }
        else
        {
            DestroyIndicator();
        }
    }

    void DestroyIndicator()
    {
        if (interactIndicator != null)
            {
                Destroy(interactIndicator);
            }
    }

    void StartTask(string taskName)
    {
        isTasking = true;
        if (taskName == "Bookshelf")
        {
            currentMinigame = taskName;
            StartBookshelf();
        }
        else if(taskName == "Diary")
        {
            currentMinigame = taskName;
            StartDiary();
        }
    }

    public void StopTask()
    {
        switch (currentMinigame)
        {
            case "Bookshelf":
            {
                StopBookshelf();
                break;
            }
            case "Diary":
            {
                StopDiary();
                break;
            }
        }
        currentMinigame = null;
    }

    void StartBookshelf()
    {
        mvmtControl.DisableMovement();
        Sequence seqBookshelfStart = DOTween.Sequence();
        seqBookshelfStart.Append(DOTween.To(()=> mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, isoViewRatio, 1f), 1));
        seqBookshelfStart.Join(DOTween.To(()=> bookshelfCam.rect, x=> bookshelfCam.rect = x, new Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f), 1));
        // seqBookshelfStart.onComplete = (()=> canvasBookshelf.gameObject.SetActive(true));
    }

    void StopBookshelf()
    {
        mvmtControl.EnableMovement();
        Sequence seqBookshelfStop = DOTween.Sequence();
        seqBookshelfStop.Append(DOTween.To(()=> mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, 1f, 1f), 1));
        seqBookshelfStop.Join(DOTween.To(()=> bookshelfCam.rect, x=> bookshelfCam.rect = x, new Rect(1f, 0, (1-isoViewRatio), 1f), 1));
        seqBookshelfStop.onComplete = ()=> isTasking = false;
    }
    void StartDiary()
    {
        mvmtControl.DisableMovement();
        Sequence seqDiaryStart = DOTween.Sequence();
        seqDiaryStart.Append(DOTween.To(()=>mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, isoViewRatio, 1f), 1));
        seqDiaryStart.Join(DOTween.To(()=> diaryCam.rect, x=> diaryCam.rect = x, new Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f), 1));
    }
    void StopDiary()
    {
        mvmtControl.EnableMovement();
        Sequence seqDiaryStop = DOTween.Sequence();
        seqDiaryStop.Append(DOTween.To(()=>mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, 1f, 1f), 1));
        seqDiaryStop.Join(DOTween.To(()=> diaryCam.rect, x=> diaryCam.rect = x, new Rect(1f, 0, (1-isoViewRatio), 1f), 1));
        seqDiaryStop.onComplete = ()=> isTasking = false;
    }
}
