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
    }

    IEnumerator DoSleepTransition()
    {
        movementController.DisableMovement();
        StartCoroutine(playerTransition.TeleportFadeInOut(movementController.currentPos));
        while (playerTransition.isTeleporting) yield return null;
        movementController.FaceDirection(Vector3Int.down);
        timeController.days += 1;
        timeController.hours += 8;
        timeController.mins = 0;
        timeController.seconds = 0;
        timeController.isShopClosed = false;
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
