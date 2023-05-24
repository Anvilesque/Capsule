using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    private Camera mainCam;
    private List<Camera> taskCams;
    private PlayerMovement mvmtControl;
    private Sprite playerSprite;
    private TileManager tileManager;
    private Tilemap interactableMap;
    public GameObject interactIndicator;
    private UIController uiController;

    private Camera bookshelfCam;
    private Camera diaryCam;

    private string currentMinigame;
    private bool isPlayerNextToTask;
    private bool isTasking;
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
        mvmtControl = FindObjectOfType<PlayerMovement>();
        playerSprite = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().sprite;
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        uiController = FindObjectOfType<UIController>();
        
        bookshelfCam = taskCams.Find(x=> x.name.Contains("Bookshelf"));
        bookshelfCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        diaryCam = taskCams.Find(x=> x.name.Contains("Diary"));
        diaryCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        isoViewRatio = 0.2f;
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
            if (!isPlayerNextToTask) {}
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
            isPlayerNextToTask = false;
            if (tileManager.ScanForTile(interactableMap, tempTile))
            {
                tempTile = tileManager.ScanForTileValue(interactableMap, tempTile);
                taskName = tileManager.GetTileData(interactableMap, tempTile).taskName;
                isPlayerNextToTask = true;
                break;
            }
        }
    }

    void IndicateInteract()
    {
        if (isTasking)
        {
            DestroyIndicator();
        }
        else if (isPlayerNextToTask)
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
        Camera taskCam = null;
        switch (taskName)
        {
            case "Bookshelf":
            {
                currentMinigame = taskName;
                taskCam = bookshelfCam;
                break;
            }
            case "Diary":
            {
                currentMinigame = taskName;
                taskCam = diaryCam;
                break;
            }
            case "Packaging":
            {
                SceneManager.LoadScene("Packaging Minigame");
                break;
            }
            default:
            {
                return;
            }
        }
        mvmtControl.DisableMovement();
        mainCam.rect = new Rect(0, 0, isoViewRatio, 1f);
        taskCam.rect = new Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f);
    }

    public void StopTask()
    {
        Camera taskCam = null;
        switch (currentMinigame)
        {
            case "Bookshelf":
            {
                taskCam = bookshelfCam;
                break;
            }
            case "Diary":
            {
                taskCam = diaryCam;
                break;
            }
            default:
            {
                return;
            }
        }
        mainCam.rect = new Rect(0, 0, 1f, 1f);
        taskCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);
        isTasking = false;
        mvmtControl.EnableMovement();
    }
}
