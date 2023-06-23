using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BedManager : MonoBehaviour
{
    private SaveManager saveManager;
    private TaskManager taskManager;
    private TimeController timeController;
    private PlayerMovement movementController;
    private PlayerTransition playerTransition;
    private BSBoxRandomManager boxRandomManager;
    private BSBoxSortedManager boxSortedManager;
    public Canvas bedCanvas;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        taskManager = FindObjectOfType<TaskManager>();
        timeController = FindObjectOfType<TimeController>();
        movementController = FindObjectOfType<PlayerMovement>();
        playerTransition = FindObjectOfType<PlayerTransition>();
        boxRandomManager = FindObjectOfType<BSBoxRandomManager>();
        boxSortedManager = FindObjectOfType<BSBoxSortedManager>();
        bedCanvas.gameObject.SetActive(false);
    }

    public void OpenSleepMenu()
    {
        timeController.canUpdateTime = false;
        bedCanvas.gameObject.SetActive(true);
    }

    public void DoSleep()
    {
        bedCanvas.gameObject.SetActive(false);
        StartCoroutine("DoSleepTransition");
        if (boxRandomManager.itemStorage.Count + boxSortedManager.itemStack.Count < 30)
        {
            boxRandomManager.AddObjects(boxRandomManager.itemsDayAll[timeController.days], boxRandomManager.itemsDayAllCount[timeController.days]);
        }
        FindObjectOfType<YarnFunctions>().ResetNPCDialogueNumber();
    }

    IEnumerator DoSleepTransition()
    {
        movementController.DisableMovement();
        StartCoroutine(playerTransition.TeleportFadeInOut(movementController.defaultBedPosition));
        while (playerTransition.isTeleporting) yield return null;
        movementController.FaceDirection(Vector3Int.down);
        timeController.days = (timeController.days + 1) % 7;
        if (timeController.days == 0) timeController.years++;
        timeController.hours = timeController.hours >= 24 ? timeController.hours % 24 + 8 :
                               timeController.hours >= 22 ? 8 :
                               6;
        timeController.mins = 0;
        timeController.seconds = 0;
        timeController.isShopClosed = false;
        timeController.isPassingOut = false;
        timeController.canUpdateTime = true;
        foreach (NPCMovement npc in FindObjectsOfType<NPCMovement>(true))
        {
            npc.gameObject.SetActive(true);
            npc.PrepareForNextDay();
        }
        taskManager.SetIsTasking(false);
        saveManager.SaveData();
        movementController.EnableMovement();
    }

    public void CloseSleepMenu()
    {
        timeController.canUpdateTime = true;
        taskManager.SetIsTasking(false);
        movementController.EnableMovement();
        bedCanvas.gameObject.SetActive(false);
    }
}
