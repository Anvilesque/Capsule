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
    public GameObject interactIndicator;
    private UIController uiController;
    private DialogueRunner dialogueRunner;
    private bool canInteract;
    private NonPC nearestNPC;

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
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (nonPCs == null) {}
        else foreach (NonPC nonPC in nonPCs)
        {
            Vector3Int nonPCCell = floorMap.WorldToCell(nonPC.position);
            Vector3Int playerCell = floorMap.WorldToCell(transform.position);
            int playerDistX = Mathf.Abs(playerCell.x - nonPCCell.x);
            int playerDistY = Mathf.Abs(playerCell.y - nonPCCell.y);
            CheckInteractNPC(playerDistX, playerDistY, nonPC);
        }
        if (canInteract)
        {
            IndicateInteract();
            AttemptInteractNPC();
        }
        else
        {
            DestroyIndicator();
        }
    }

    void CheckInteractNPC(int playerDistX, int playerDistY, NonPC nonPC)
    {
        if (dialogueRunner.IsDialogueRunning) return; //dialogue is running
        if (playerDistX <= 1 && playerDistY <= 1)
        {
            nearestNPC = nonPC;
            canInteract = true;
        }
        else
        {
            canInteract = false;
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
            canInteract = false;
            mvmtControl.DisableMovement();
            dialogueRunner.StartDialogue(nearestNPC.introTitle);
        }
    }

    void AfterDialogue()
    {
        mvmtControl.EnableMovement();
    }
}
