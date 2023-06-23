using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Yarn.Unity;

public class PlayerNPCEncounter : MonoBehaviour
{
    private List<NonPC> nonPCs;
    private PlayerMovement mvmtControl;
    private TileManager tileManager;
    private Tilemap floorMap;
    private TaskManager taskManager;
    private TimeController timeController;
    private GameObject interactIndicator;
    private UIController uiController;
    private DialogueRunner dialogueRunner;
    public bool canInteractNPC {get; private set;}
    public bool hasInteractNPCPriority {get; private set;}
    private NonPC nearestNPC;
    private Vector3Int nearestNPCDirection;

    // Start is called before the first frame update
    void Start()
    {
        nonPCs = new List<NonPC>(FindObjectsOfType<NonPC>(true));
        mvmtControl = GetComponent<PlayerMovement>();
        uiController = FindObjectOfType<UIController>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        taskManager = FindObjectOfType<TaskManager>();
        timeController = FindObjectOfType<TimeController>();

        if (dialogueRunner == null) {}
        else dialogueRunner.onDialogueComplete.AddListener(AfterDialogue);
        canInteractNPC = false;
        hasInteractNPCPriority = false;
    }

    // Update is called once per frame
    void Update()
    {
        canInteractNPC = false;
        if (nonPCs.Count == 0) return;
        else UpdateCanInteract();
        
        if (canInteractNPC)
        {
            IndicateInteract();
            AttemptInteractNPC();
        }
        else
        {
            DestroyIndicator();
        }
    }

    void UpdateCanInteract()
    {
        if (dialogueRunner.IsDialogueRunning) return; //dialogue is running
        if (CapsuleResponseViewer.isWriting) return;
        Vector3Int playerCell = mvmtControl.currentPos;
        NonPC adjacentNPC = GetNPCAtAdjacent(playerCell);
        if (adjacentNPC != null && adjacentNPC.enabled)
        {
            nearestNPC = adjacentNPC;
            nearestNPCDirection = GetNPCDirection(playerCell, nearestNPC);
            canInteractNPC = true;
        }
        else canInteractNPC = false;
        
    }

    public NonPC GetNPCAtAdjacent(Vector3Int playerPosition)
    {
        NonPC nonPCDirectlyFacing = GetNPCAtPosition(playerPosition + mvmtControl.currentlyFacing);
        if (nonPCDirectlyFacing != null)
        {
            hasInteractNPCPriority = true;
            return nonPCDirectlyFacing;
        }
        hasInteractNPCPriority = false;
        List<Vector3Int> cellsAdjacentToPlayer = tileManager.GetAdjacentCellsPositions(floorMap, playerPosition);
        return nonPCs.Find(nonPC => cellsAdjacentToPlayer.Contains(nonPC.position));
    }

    public NonPC GetNPCAtPosition(Vector3Int targetPosition)
    {
        return nonPCs.Find(nonPC => nonPC.position == targetPosition && nonPC.enabled == true);
    }

    Vector3Int GetNPCDirection(Vector3Int playerPosition, NonPC nonPC)
    {
        List<Vector3Int> cellsAdjacentToPlayer = tileManager.GetAdjacentCellsPositions(floorMap, playerPosition);
        return TileManager.cardinalDirections[cellsAdjacentToPlayer.IndexOf(nonPC.position)];
    }

    void IndicateInteract()
    {
        if (interactIndicator != null) return;
        interactIndicator = Instantiate((GameObject)Resources.Load("Prefabs/InteractIndicator"), mvmtControl.transform.position, Quaternion.identity, mvmtControl.transform);
        interactIndicator.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        interactIndicator.transform.localPosition = new Vector3(-0.5f, 1f, 100f);
    }

    void DestroyIndicator()
    {
        if (interactIndicator != null)
        {
            Destroy(interactIndicator);
        }
    }

    void AttemptInteractNPC()
    {
        if (taskManager.isTasking) return;
        if (taskManager.isPlayerNextToTask && !hasInteractNPCPriority) return;
        if (Input.GetButtonDown("Interact"))
        {
            canInteractNPC = false;
            timeController.canUpdateTime = false;
            mvmtControl.DisableMovement();
            mvmtControl.FaceDirection(nearestNPCDirection);
            nearestNPC.GetComponent<NPCMovement>().FaceDirection(-nearestNPCDirection);
            dialogueRunner.StartDialogue(nearestNPC.introTitle);
            FindObjectOfType<HUDButtons>().DisableHUD();
        }
    }

    void AfterDialogue()
    {
        if (CapsuleResponseViewer.isWriting) return;
        if (timeController.isShopClosed) return;
        mvmtControl.EnableMovement();
        timeController.canUpdateTime = true;
        FindObjectOfType<HUDButtons>().EnableHUD();
    }
}
