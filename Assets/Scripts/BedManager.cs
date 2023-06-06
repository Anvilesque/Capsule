using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BedManager : MonoBehaviour
{
    private Canvas bedCanvas;
    private TaskManager taskManager;
    private TimeController timeController;
    private PlayerMovement movementController;
    private PlayerTransition playerTransition;

    // Start is called before the first frame update
    void Start()
    {
        bedCanvas = new List<Canvas>(FindObjectsOfType<Canvas>()).Find(canvas => canvas.name.Contains("Bed"));
        bedCanvas.gameObject.SetActive(false);
        taskManager = FindObjectOfType<TaskManager>();
        timeController = FindObjectOfType<TimeController>();
        movementController = FindObjectOfType<PlayerMovement>();
        playerTransition = FindObjectOfType<PlayerTransition>();
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
        BSBoxRandomManager boxRandomManager = FindObjectOfType<BSBoxRandomManager>();
        boxRandomManager.AddObjects(boxRandomManager.itemsDayAll[timeController.days - 1], boxRandomManager.itemsDayAllCount[timeController.days - 1]);
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
        if (timeController.hours >= 8 && timeController.hours <= 20) timeController.hours = 6;
        else if (timeController.hours >= 21) timeController.hours = 8;
        else timeController.hours += 8;
        timeController.mins = 0;
        timeController.seconds = 0;
        timeController.SaveTime();
        timeController.isShopClosed = false;
        timeController.isPassingOut = false;
        timeController.canUpdateTime = true;
        foreach (NPCMovement npc in FindObjectsOfType<NPCMovement>())
        {
            npc.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            npc.Teleport(true);
        }
        taskManager.SetIsTasking(false);
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
