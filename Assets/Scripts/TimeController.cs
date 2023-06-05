using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // using text mesh for the clock display
using Yarn.Unity;

public class TimeController : MonoBehaviour
{
    
    // public TextMeshProUGUI timeDisplay; // Display Time
    // public TextMeshProUGUI dayDisplay; // Display Day
    public string timeTextTime {get; private set;}
    public string timeTextDay {get; private set;}
    private int displayInterval; // Changes how often display text updates
    
    public float tick; // Increasing the tick, increases second rate
    public float seconds; 
    public int mins;
    public int hours;
    public int days = 1;
    private int shopCloseHour;
    private DialogueRunner dialogueRunner;
    private PlayerMovement movementController;
    private PlayerTransition playerTransition;
    [SerializeField] private float closeShopWindowDuration;
    public bool isShopClosed;
    public bool canUpdateTime;
 
    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        movementController = FindObjectOfType<PlayerMovement>();
        playerTransition = FindObjectOfType<PlayerTransition>();
        displayInterval = 30;
        shopCloseHour = 21;
        canUpdateTime = true;
    }
 
    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame dependant. 
    {
        if (canUpdateTime)
        {
            CalcTime();
            UpdateTimeText();
        }
        if (isShopClosed) return;
        else if (hours >= shopCloseHour)
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
 
        if (hours >= 24) //24 hr = 1 day
        {
            // Days will be handled manually in BedManager
            // days += hours / 24;
            hours = hours % 24;
        }
    }
 
    public void UpdateTimeText() // Shows time and day in ui
    {
        timeTextTime = string.Format("{0:00}:{1:00}", hours, (int)(mins / displayInterval) * displayInterval); // The formatting ensures that there will always be 0's in empty spaces
        timeTextDay = "Day " + days; // display day counter
    }

    public IEnumerator CloseShop() // checks the current time and performs events at a certain time
    {
        canUpdateTime = false;
        isShopClosed = true;
        movementController.DisableMovement();
        dialogueRunner.StartDialogue("close_shop");
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
        movementController.EnableMovement();
    }
}