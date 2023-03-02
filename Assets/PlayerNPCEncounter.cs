using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerNPCEncounter : MonoBehaviour
{
    private List<NonPC> nonPCs;
    private DialogueRunner dialogueRunner;
    private bool canInteract;

    // Start is called before the first frame update
    void Start()
    {
        nonPCs = new List<NonPC>(FindObjectsOfType<NonPC>());
        dialogueRunner = FindObjectOfType<DialogueRunner>();
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
            CheckInteractNPC(cellDistX, cellDistY);
            InteractNPC(nonPC);
        }
    }

    void CheckInteractNPC(int cellDistX, int cellDistY)
    {
        if (dialogueRunner.IsDialogueRunning) {} //dialogue is running
        else if (cellDistX <= 1 && cellDistY <= 1)
        {
            canInteract = true;
        }
    }

    void InteractNPC(NonPC nonPC)
    {
        if (Input.GetButton("Interact") && canInteract)
        {
            canInteract = false;
            dialogueRunner.StartDialogue(nonPC.introTitle);
        }
    }
}
