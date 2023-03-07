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

    // Start is called before the first frame update
    void Start()
    {
        nonPCs = new List<NonPC>(FindObjectsOfType<NonPC>());
        mvmtControl = GetComponent<PlayerMovementController>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (dialogueRunner == null) {}
        else dialogueRunner.onDialogueComplete.AddListener(MovementAfterDialogue);
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (nonPCs == null) {}
        else foreach (NonPC nonPC in nonPCs)
        {
            Vector3Int nonPCCell = TileManager.WorldCoordsToGridCoords(nonPC.position);
            Vector3Int playerCell = TileManager.WorldCoordsToGridCoords(transform.position);
            int playerDistX = Mathf.Abs(playerCell.x - nonPCCell.x);
            int playerDistY = Mathf.Abs(playerCell.y - nonPCCell.y);
            CheckInteractNPC(playerDistX, playerDistY, nonPC);
        }
        InteractNPC();
    }

    void CheckInteractNPC(int playerDistX, int playerDistY, NonPC nonPC)
    {
        if (dialogueRunner.IsDialogueRunning) {} //dialogue is running
        else if (playerDistX <= 1 && playerDistY <= 1)
        {
            nearestNPC = nonPC;
            canInteract = true;
        }
        else
        {
            canInteract = false;
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
