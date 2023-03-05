using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class PlayerNPCEncounter : MonoBehaviour
{
    private List<NonPC> nonPCs;
    private PlayerMovementController mvmtControl;
    private DialogueRunner dialogueRunner;
    private bool canInteract;
    private NonPC nearestNPC;
    public bool isInteracting {get; private set;}
    private UnityAction ActionMovement;

    // Start is called before the first frame update
    void Start()
    {
        nonPCs = new List<NonPC>(FindObjectsOfType<NonPC>());
        mvmtControl = GetComponent<PlayerMovementController>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        ActionMovement += MovementAfterDialogue;
        dialogueRunner.onDialogueComplete.AddListener(MovementAfterDialogue);
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (NonPC nonPC in nonPCs)
        {
            Vector3Int nonPCCell = TileManager.WorldCoordsToGridCoords(nonPC.position);
            Vector3Int playerCell = TileManager.WorldCoordsToGridCoords(transform.position);
            int cellDistX = Mathf.Abs(playerCell.x - nonPCCell.x);
            int cellDistY = Mathf.Abs(playerCell.y - nonPCCell.y);
            CheckInteractNPC(cellDistX, cellDistY, nonPC);
        }
        InteractNPC();
    }

    void CheckInteractNPC(int cellDistX, int cellDistY, NonPC nonPC)
    {
        if (dialogueRunner.IsDialogueRunning) {} //dialogue is running
        else if (cellDistX <= 1 && cellDistY <= 1)
        {
            nearestNPC = nonPC;
            canInteract = true;
        }
    }

    void InteractNPC()
    {
        if (Input.GetButtonDown("Interact") && canInteract)
        {
            canInteract = false;
            mvmtControl.DisableMovement();
            dialogueRunner.StartDialogue(nearestNPC.introTitle);
        }
    }

    void MovementAfterDialogue()
    {
        mvmtControl.EnableMovement();
    }
}
