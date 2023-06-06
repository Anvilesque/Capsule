using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

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
    private Camera cleaningCam;
    private Camera capsuleCam;

    private string currentTask;
    private bool isPlayerNextToTask;
    public bool isTasking {get; private set;}
    private string taskName;
    private Vector3Int lastInteractDirection;
    private float isoViewRatio;
    private TMP_InputField diaryInput;
    private GameObject player;
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
        player = GameObject.FindWithTag("Player");
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        uiController = FindObjectOfType<UIController>();
        
        bookshelfCam = taskCams.Find(x=> x.name.Contains("Bookshelf"));
        bookshelfCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        diaryCam = taskCams.Find(x=> x.name.Contains("Diary"));
        diaryCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        cleaningCam = taskCams.Find(x=> x.name.Contains("Cleaning"));
        cleaningCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        capsuleCam = taskCams.Find(x=> x.name.Contains("Capsule View"));
        capsuleCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);

        lastInteractDirection = Vector3Int.zero;

        isoViewRatio = 0.2f;
        // canvasBookshelf.gameObject.SetActive(false);
        DOTween.Init();
        diaryInput = FindObjectOfType<SaveDiary>().transform.root.GetComponentInChildren<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractables();
        IndicateInteract();
        if (Input.GetButtonDown("Interact"))
        {
            if (!isPlayerNextToTask) return;
            if (InputFieldManager.isInputFocused) return;
            if (FindObjectOfType<PlayerNPCEncounter>().GetNPCAtAdjacent(mvmtControl.currentPos) != null) return;
            if (isTasking)
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
        if (isTasking) return;
        List<Vector3Int> cardinalDirections = new List<Vector3Int>() {Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down};
        foreach (Vector3Int direction in cardinalDirections)
        {
            Vector3Int tempTile = mvmtControl.currentPos + direction;
            isPlayerNextToTask = false;
            if (tileManager.ScanForTile(interactableMap, tempTile))
            {
                tempTile = tileManager.GetTilePosition(interactableMap, tempTile);
                taskName = tileManager.GetTileData(interactableMap, tempTile).taskName;
                lastInteractDirection = direction;
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
                currentTask = taskName;
                taskCam = bookshelfCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            case "Diary":
            {
                currentTask = taskName;
                taskCam = diaryCam;
                diaryInput.enabled = true;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            case "Cleaning":
            {
                currentTask = taskName;
                taskCam = cleaningCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            case "Capsule":
            {
                currentTask = taskName;
                taskCam = capsuleCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            case "Packaging":
            {
                SceneManager.LoadScene("Packaging Minigame");
                PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
                PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
                PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
                PlayerPrefs.SetInt("DirX", mvmtControl.currentlyFacing.x);
                PlayerPrefs.SetInt("DirY", mvmtControl.currentlyFacing.y);
                break;
            }
            case "Bed":
            {
                currentTask = null;
                FindObjectOfType<BedManager>().OpenSleepMenu();
                break;
            }
            default:
            {
                return;
            }
        }
        mvmtControl.DisableMovement();
        mvmtControl.FaceDirection(lastInteractDirection);
        if (taskCam != null)
        {
            mainCam.rect = new Rect(0, 0, isoViewRatio, 1f);
            taskCam.rect = new Rect(isoViewRatio, 0, (1 - isoViewRatio), 1f);
        }
        
    }

    public void StopTask()
    {
        Camera taskCam = null;
        switch (currentTask)
        {
            case "Bookshelf":
            {
                taskCam = bookshelfCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            case "Diary":
            {
                taskCam = diaryCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            case "Cleaning":
            {
                taskCam = cleaningCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                FindObjectOfType<CleaningManager>().ResetMinigame();
                break;
            }
            case "Capsule":
            {
                taskCam = capsuleCam;
                FindObjectOfType<HUDButtons>().ToggleHUD();
                break;
            }
            default:
            {
                return;
            }
        }
        if (taskCam != null)
        {
            mainCam.rect = new Rect(0, 0, 1f, 1f);
            taskCam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);
        }
        isTasking = false;
        mvmtControl.EnableMovement();
    }

    public void SetIsTasking(bool isTasking)
    {
        this.isTasking = isTasking;
    }
}
