using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // using text mesh for the clock display
 
public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay; // Display Time
    public TextMeshProUGUI dayDisplay; // Display Day
 
    public float tick; // Increasing the tick, increases second rate
    public float seconds; 
    public int mins;
    public int hours;
    public int days = 1;
    private GameObject player;
    public PlayerMovementController movementController;
    public float newX;
    public float newY;
 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        movementController = player.GetComponent<PlayerMovementController>();
    }
 
    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame dependant. 
    {
        CalcTime();
        DisplayTime();
        StartCoroutine(HandleEvents());
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
 
    public void DisplayTime() // Shows time and day in ui
    {
        timeDisplay.text = string.Format("{0:00}:{1:00}", hours, mins); // The formatting ensures that there will always be 0's in empty spaces
        dayDisplay.text = "Day: " + days; // display day counter
    }

    private IEnumerator HandleEvents() // checks the current time and performs events at a certain time
    {
        // check for shop closing and teleport player out if closed
        if (hours == 21 && mins == 0 && seconds == 0) // check for whole minute so that transform happen even if the player is moving
        {
            movementController.disableMovement();
            yield return new WaitForSeconds(movementController.timeToMove); // wait for movement to finish to tp and reenable movement
            player.transform.position = new Vector3(newX,newY,0); // teleport player outside of shop
            movementController.enableMovement();
        }
    }
}