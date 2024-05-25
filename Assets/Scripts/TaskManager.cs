using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class TaskManager : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    private GameObject player;
    private PlayerMovement mvmtControl;
    private PlayerNPCEncounter playerNPCEncounter;
    private Sprite playerSprite;
    private TileManager tileManager;
    private Tilemap interactableMap;
    private UIController uiController;
    private HUDButtons hudButtons;
    public GameObject interactIndicator;

    [Header("Cameras")]
    private Camera mainCam;
    private List<Camera> taskCams;
    public Camera bookshelfCam;
    public Camera diaryCam;
    public Camera cleaningCam;
    public Camera capsuleCam;
    private float isoViewRatio = 0.2f;

    private BedManager bedManager;
    private CleaningManager cleaningManager;
    private CapsuleViewManager capsuleViewManager;

    public string currentTask {get; private set;}
    public bool isPlayerNextToTask {get; private set;}
    private Vector3Int lastInteractDirection;
    public bool isTasking {get; private set;}
    private string taskName;
    private TMP_InputField diaryInput;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        taskCams = new List<Camera>();
        foreach (Camera cam in new Camera[] {bookshelfCam, diaryCam, cleaningCam, capsuleCam})
        {
            taskCams.Add(cam);
            cam.transparencySortMode = TransparencySortMode.Default;
            cam.rect = new Rect(1f, 0, (1 - isoViewRatio), 1f);
        }
        
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        bedManager = FindObjectOfType<BedManager>(true);
        cleaningManager = FindObjectOfType<CleaningManager>(true);
        capsuleViewManager = FindObjectOfType<CapsuleViewManager>(true);

        player = GameObject.FindWithTag("Player");
        mvmtControl = player.GetComponent<PlayerMovement>();
        playerSprite = player.GetComponent<SpriteRenderer>().sprite;
        playerNPCEncounter = player.GetComponent<PlayerNPCEncounter>();
        tileManager = FindObjectOfType<TileManager>();
        interactableMap = tileManager.interactableMap;
        uiController = FindObjectOfType<UIController>();
        hudButtons = FindObjectOfType<HUDButtons>();        

        lastInteractDirection = Vector3Int.zero;

        // canvasBookshelf.gameObject.SetActive(false);
        diaryInput = FindObjectOfType<SaveDiary>().transform.root.GetComponentInChildren<TMP_InputField>();
        diaryInput.enabled = false;
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
            if (CapsuleResponseViewer.isWriting) return;
            if (playerNPCEncounter.hasInteractNPCPriority) return;
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
        isPlayerNextToTask = false;
        CheckInteractablesHelper(mvmtControl.currentlyFacing);
        if (isPlayerNextToTask) return;

        foreach (Vector3Int direction in TileManager.cardinalDirections)
        {
            if (direction == mvmtControl.currentlyFacing) continue;
            CheckInteractablesHelper(direction);
            if (isPlayerNextToTask) return;
        }
    }

    private void CheckInteractablesHelper(Vector3Int direction)
    {
        Vector3Int tempTile = mvmtControl.currentPos + direction;
        if (tileManager.ScanForTile(interactableMap, tempTile))
        {
            tempTile = tileManager.GetTilePosition(interactableMap, tempTile);
            taskName = tileManager.GetTileData(interactableMap, tempTile).taskName;
            lastInteractDirection = direction;
            isPlayerNextToTask = true;
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
                hudButtons.DisableHUD();
                break;
            }
            case "Diary":
            {
                currentTask = taskName;
                taskCam = diaryCam;
                diaryInput.enabled = true;
                hudButtons.DisableHUD();
                break;
            }
            case "Cleaning":
            {
                
                currentTask = taskName;
                taskCam = cleaningCam;
                hudButtons.DisableHUD();
                break;
            }
            case "Capsule":
            {
                capsuleViewManager.ResetCapsuleView();
                currentTask = taskName;
                taskCam = capsuleCam;
                hudButtons.DisableHUD();
                break;
            }
            case "Packaging":
            {
                saveManager.UpdateDataPlayer();
                saveManager.SaveData(false);
                SceneManager.LoadScene("Packaging Minigame");
                break;
            }
            case "Bed":
            {
                currentTask = null;
                bedManager.OpenSleepMenu();
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
                hudButtons.EnableHUD();
                break;
            }
            case "Diary":
            {
                taskCam = diaryCam;
                diaryInput.enabled = false;
                hudButtons.EnableHUD();
                break;
            }
            case "Cleaning":
            {
                taskCam = cleaningCam;
                hudButtons.EnableHUD();
                cleaningManager.ResetMinigame();
                break;
            }
            case "Capsule":
            {
                taskCam = capsuleCam;
                hudButtons.EnableHUD();
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
        saveManager.SaveData();
        mvmtControl.EnableMovement();
    }

    public void SetIsTasking(bool isTasking)
    {
        this.isTasking = isTasking;
    }
}
