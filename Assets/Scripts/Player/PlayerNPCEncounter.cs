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
    private GameObject interactIndicator;
    private UIController uiController;
    private DialogueRunner dialogueRunner;
    private bool canInteractNPC;
    private NonPC nearestNPC;
    private Vector3Int nearestNPCDirection;

    // Start is called before the first frame update
    void Start()
    {
        nonPCs = new List<NonPC>(FindObjectsOfType<NonPC>());
        mvmtControl = GetComponent<PlayerMovement>();
        uiController = FindObjectOfType<UIController>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;

        if (dialogueRunner == null) {}
        else dialogueRunner.onDialogueComplete.AddListener(AfterDialogue);
        canInteractNPC = false;
    }

    // Update is called once per frame
    void Update()
    {
        canInteractNPC = false;
        if (nonPCs.Count == 0) {}
        else foreach (NonPC nonPC in nonPCs)
        {
            Vector3Int playerCell = mvmtControl.currentPos;
            CheckInteractNPC(playerCell, nonPC);
            if (canInteractNPC) break;
        }
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

    void CheckInteractNPC(Vector3Int playerPosition, NonPC nonPC)
    {
        if (dialogueRunner.IsDialogueRunning) return; //dialogue is running
        List<Vector3Int> cellsAdjacentToPlayer = tileManager.GetAdjacentCellsPositions(floorMap, playerPosition);
        if (cellsAdjacentToPlayer.Contains(nonPC.position))
        {
            nearestNPC = nonPC;
            nearestNPCDirection = TileManager.cardinalDirections[cellsAdjacentToPlayer.IndexOf(nonPC.position)];
            canInteractNPC = true;
        }
        else
        {
            canInteractNPC = false;
        }
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
        if (Input.GetButtonDown("Interact"))
        {
            canInteractNPC = false;
            mvmtControl.DisableMovement();
            mvmtControl.FaceDirection(nearestNPCDirection);
            dialogueRunner.StartDialogue(nearestNPC.introTitle);
        }
    }

    void AfterDialogue()
    {
        mvmtControl.EnableMovement();
    }
}
