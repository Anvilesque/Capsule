using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // using text mesh for the clock display
using Yarn.Unity;

public class TimeController : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    // public TextMeshProUGUI timeDisplay; // Display Time
    // public TextMeshProUGUI dayDisplay; // Display Day
    public string timeTextTime {get; private set;}
    public string timeTextDay {get; private set;}
    private int displayInterval = 10; // Changes how often display text updates
    
    public float tick; // Increasing the tick, increases second rate
    [HideInInspector] public float seconds;
    [HideInInspector] public int mins;
    [HideInInspector] public int hours;
    [HideInInspector] public int days;
    [HideInInspector] public int years;
    private int hourShopClose = 21;
    private int hourPassOut = 29;
    private TaskManager taskManager;
    private DialogueRunner dialogueRunner;
    private PlayerMovement movementController;
    private PlayerTransition playerTransition;
    private float closeShopWindowDuration = 3f;
    public bool isPassingOut;
    public bool isShopClosed;
    public bool canUpdateTime;
 
    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        taskManager = FindObjectOfType<TaskManager>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        movementController = FindObjectOfType<PlayerMovement>();
        playerTransition = FindObjectOfType<PlayerTransition>();
        canUpdateTime = true;
        // canUpdateTime = saveData.canUpdateTime;
        isShopClosed = saveData.isShopClosed;
        isPassingOut = false;
        seconds = saveData.seconds;
        mins = saveData.mins;
        hours = saveData.hours;
        days = saveData.days;
        years = saveData.years;
    }
 
    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame-dependent.
    {
        if (canUpdateTime)
        {
            CalcTime();
            UpdateTimeText();
        }
        if (isPassingOut) {}
        else if (hours == hourPassOut)
        {
            StartCoroutine(PassOut());
            return;
        }
        if (isShopClosed) {}
        else if (hours >= hourShopClose)
        {
            StartCoroutine(CloseShop());
        }
    }

    public void CalcTime() // Used to calculate sec, min and hours
    {
        seconds += Time.fixedDeltaTime * tick; // multiply time between fixed update by tick
 
        if (seconds >= 60) // 60 sec = 1 min
        {
            mins += Mathf.FloorToInt(seconds / 60);
            seconds = seconds % 60;
        }
 
        if (mins >= 60) //60 min = 1 hr
        {
            hours += mins / 60;
            mins = mins % 60;
        }
 
        // Hours can go past 24, for late wake-up mechanic
        // Days handled manually in BedManager
    }
 
    public void UpdateTimeText() // Shows time and day in ui
    {
        timeTextTime = string.Format("{0:00}:{1:00}", hours % 24, (int)(mins / displayInterval) * displayInterval); // The formatting ensures that there will always be 0's in empty spaces
        switch (days)
        {
            case 0: { timeTextDay = "New Year's Day"; break; }
            case 1: { timeTextDay = "Valentine's Day"; break; }
            case 2: { timeTextDay = "Graduation Day"; break; }
            case 3: { timeTextDay = "Fourth of July"; break; }
            case 4: { timeTextDay = "Halloween"; break; }
            case 5: { timeTextDay = "Thanksgiving"; break; }
            case 6: { timeTextDay = "Christmas"; break; }
        }
    }

    public IEnumerator CloseShop() // checks the current time and performs events at a certain time
    {
        canUpdateTime = false;
        isShopClosed = true;
        movementController.DisableMovement();
        taskManager.StopTask();
        FindObjectOfType<HUDButtons>().DisableHUD();
        if (playerTransition.isInRoom) dialogueRunner.StartDialogue("close_shop_from_room");
        else dialogueRunner.StartDialogue("close_shop_from_shop");
        yield return new WaitForSeconds(closeShopWindowDuration);
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.OnViewRequestedInterrupt();
        }
        StartCoroutine(playerTransition.TeleportFadeInOut(movementController.defaultHomePosition));
        while (playerTransition.isTeleporting) yield return null;
        hours = 22;
        mins = 0;
        seconds = 0;
        canUpdateTime = true;
        saveManager.SaveData();
        FindObjectOfType<HUDButtons>().EnableHUD();
    }

     public IEnumerator PassOut() // checks the current time and performs events at a certain time
    {
        canUpdateTime = false;
        isPassingOut = true;
        movementController.DisableMovement();
        dialogueRunner.StartDialogue("pass_out");
        while (dialogueRunner.IsDialogueRunning) yield return null;
        FindObjectOfType<BedManager>().DoSleep();
    }
}