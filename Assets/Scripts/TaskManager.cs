using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TaskManager : MonoBehaviour
{
    private Camera mainCam;
    private List<Camera> taskCams;
    private PlayerMovementController mvmtControl;
    private Sprite playerSprite;
    private TileManager tileManager;
    private Tilemap interactableMap;
    public GameObject interactIndicator;
    private UIController uiController;

    private Camera bookshelfCam;
    private Camera diaryCam;

    private string currentMinigame;
    private bool taskAvailable;
    private bool isTasking;
    private bool isProcessing;
    private string taskName;
    private float isoViewRatio;
    // public Canvas canvasBookshelf;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        taskCams = new List<Camera>();
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam == Camera.main) continue;
            else
            {
                taskCams.Add(cam);
                cam.transparencySortMode = TransparencySortMode.Default;
            }
        }
        mvmtControl = FindObjectOfType<PlayerMovementController>();
        playerSprite = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().sprite;
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        uiController = FindObjectOfType<UIController>();
        
        bookshelfCam = taskCams.Find(x=> x.name.Contains("Bookshelf"));
        bookshelfCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        diaryCam = taskCams.Find(x=> x.name.Contains("Diary"));
        diaryCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        isoViewRatio = 0.2f;
        isTasking = false;
        isProcessing = false;
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
            else if (isProcessing) {}
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
        Vector3Int tileUp = mvmtControl.currentPos + new Vector3Int(0, -1, 0);
        Vector3Int tileDown = mvmtControl.currentPos + new Vector3Int(0, +1, 0);
        Vector3Int tileLeft = mvmtControl.currentPos + new Vector3Int(-1, 0, 0);
        Vector3Int tileRight = mvmtControl.currentPos + new Vector3Int(+1, 0, 0);
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
        isProcessing = true;
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
        isProcessing = true;
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
    }

    void StartBookshelf()
    {
        mvmtControl.DisableMovement();
        Sequence seqBookshelfStart = DOTween.Sequence();
        seqBookshelfStart.Append(DOTween.To(()=> mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, isoViewRatio, 1f), 1));
        seqBookshelfStart.Join(DOTween.To(()=> bookshelfCam.rect, x=> bookshelfCam.rect = x, new Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f), 1));
        seqBookshelfStart.AppendCallback(()=> uiController.TranslateHUD(false, 1f));
        seqBookshelfStart.AppendInterval(1f);
        seqBookshelfStart.AppendCallback(()=> isProcessing = false);
    }

    void StopBookshelf()
    {
        uiController.TranslateHUD(true, 0.5f);
        Sequence seqBookshelfStop = DOTween.Sequence();
        seqBookshelfStop.SetDelay(0.5f);
        seqBookshelfStop.Append(DOTween.To(()=> mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, 1f, 1f), 1));
        seqBookshelfStop.Join(DOTween.To(()=> bookshelfCam.rect, x=> bookshelfCam.rect = x, new Rect(1f, 0, (1 - isoViewRatio), 1f), 1));
        seqBookshelfStop.InsertCallback(1f, ()=> mvmtControl.EnableMovement());
        seqBookshelfStop.onComplete = ()=> isTasking = false;
        seqBookshelfStop.AppendCallback(()=> isProcessing = false);
    }
    void StartDiary()
    {
        mvmtControl.DisableMovement();
        Sequence seqDiaryStart = DOTween.Sequence();
        seqDiaryStart.Append(DOTween.To(()=>mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, isoViewRatio, 1f), 1));
        seqDiaryStart.Join(DOTween.To(()=> diaryCam.rect, x=> diaryCam.rect = x, new Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f), 1));
        seqDiaryStart.AppendCallback(()=> uiController.TranslateHUD(false, 1f));
        seqDiaryStart.AppendInterval(1f);
        seqDiaryStart.AppendCallback(()=> isProcessing = false);
    }
    void StopDiary()
    {
        if (InputFieldManager.isInputFocused) return;
        uiController.TranslateHUD(true, 0.5f);
        Sequence seqDiaryStop = DOTween.Sequence();
        seqDiaryStop.SetDelay(0.5f);
        seqDiaryStop.Append(DOTween.To(()=>mainCam.rect, x=> mainCam.rect = x, new Rect(0, 0, 1f, 1f), 1));
        seqDiaryStop.Join(DOTween.To(()=> diaryCam.rect, x=> diaryCam.rect = x, new Rect(1f, 0, (1 - isoViewRatio), 1f), 1));
        seqDiaryStop.InsertCallback(1f, ()=> mvmtControl.EnableMovement());
        seqDiaryStop.onComplete = ()=> isTasking = false;
        seqDiaryStop.AppendCallback(()=> isProcessing = false);
    }
}
