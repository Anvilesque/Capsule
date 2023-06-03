using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // using text mesh for the clock display
 
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
    private PlayerMovement movementController;
    private PlayerTransition playerTransition;
    [SerializeField] private float closeShopWindowDuration;
    public bool isShopClosed;
 
    // Start is called before the first frame update
    void Start()
    {
        movementController = FindObjectOfType<PlayerMovement>();
        playerTransition = FindObjectOfType<PlayerTransition>();
        displayInterval = 30;
        shopCloseHour = 21;
    }
 
    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame dependant. 
    {
        if (isShopClosed) return;
        else if (hours >= shopCloseHour)
        {
            StartCoroutine(CloseShop());
        }
        else
        {
            CalcTime();
            UpdateTimeText();
        }
    }
 
    public void CalcTime() // Used to calculate sec, min and hours
    {
        seconds += Time.fixedDeltaTime * tick; // multiply time between fixed update by tick
 
        if (seconds >= 60) // 60 sec = 1 min
        {
            seconds = 0;
            mins += 1;
        }
 
        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
        }
 
        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
        }
    }
 
    public void UpdateTimeText() // Shows time and day in ui
    {
        timeTextTime = string.Format("{0:00}:{1:00}", hours, (int)(mins / displayInterval) * displayInterval); // The formatting ensures that there will always be 0's in empty spaces
        timeTextDay = "Day " + days; // display day counter
    }

    private IEnumerator CloseShop() // checks the current time and performs events at a certain time
    {
        isShopClosed = true;
        movementController.DisableMovement();
        Debug.Log("Shop's closed! Time to go home.");
        yield return new WaitForSeconds(closeShopWindowDuration);
        playerTransition.isTeleporting = true;
        StartCoroutine(playerTransition.TeleportFadeInOut(movementController.defaultHomePosition));
        while (playerTransition.isTeleporting) yield return null;
        hours = 22;
        mins = 0;
        seconds = 0;
        movementController.EnableMovement();
    }
}