using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapsuleResponseSaver : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button saveResponseButton;
    public TMP_Text questionText;
    public GameObject inputField;
    private TaskManager taskManager;
    private TimeController time;
    public List<Dictionary<string, string>> previousResponses = new List<Dictionary<string, string>>();
    public GameObject saveResponseText;
    [HideInInspector] public int numResponses;
    [HideInInspector] public int currentResponse;

    #region Entry Tags
    public const string tagResponseIRLDate = "responseirldate";
    public const string tagResponseIRLTime = "responseirltime";
    public const string tagResponseGameDay = "responsegametime";
    public const string tagResponseGameYear = "responsegameyear";
    public const string tagResponseGameTime = "responsegamedate";
    public const string tagResponseGameQuestion = "responsegamequestion";
    public const string tagResponseGameText = "responsegametext";
    #endregion

    void Start()
    {
        numResponses = PlayerPrefs.GetInt("numResponseEntries", 0);
        for (int i = 0; i < numResponses; i++)
        {
            previousResponses.Add(new Dictionary<string, string>(){
                { tagResponseIRLDate, PlayerPrefs.GetString($"{tagResponseIRLDate}{i}") },
                { tagResponseIRLTime, PlayerPrefs.GetString($"{tagResponseIRLTime}{i}") },
                { tagResponseGameDay, PlayerPrefs.GetString($"{tagResponseGameDay}{i}") },
                { tagResponseGameYear, PlayerPrefs.GetString($"{tagResponseGameYear}{i}") },
                { tagResponseGameTime, PlayerPrefs.GetString($"{tagResponseGameTime}{i}") },
                { tagResponseGameQuestion, PlayerPrefs.GetString($"{tagResponseGameQuestion}{i}") },
                { tagResponseGameText, PlayerPrefs.GetString($"{tagResponseGameText}{i}") },
            });
        }
        currentResponse = PlayerPrefs.GetInt("currentResponse");
        taskManager = FindObjectOfType<TaskManager>();
        time = FindObjectOfType<TimeController>();
        // Calls Save when button is clicked
        saveResponseButton.onClick.AddListener(SaveResponse);
        saveResponseText.SetActive(false);
        inputField.SetActive(true);
    }

    public void SaveResponse()
    {
        string questionText = this.questionText.text;
        string responseText = inputField.GetComponent<TMP_InputField>().text;
        // No text to save --> return
        if (responseText == "")
            return;

        // Set up separately because these strings are multi-part
        string irlDate = $"{System.DateTime.Now.Month}/{System.DateTime.Now.Day}/{System.DateTime.Now.Year}";
        string gameTime = $"{time.hours.ToString("00")}:{time.mins.ToString("00")}";

        // Tag all data separately, concatenate them into one string
        Dictionary<string, string> newEntry = new Dictionary<string, string>(){
            {tagResponseIRLDate, irlDate},
            {tagResponseIRLTime, System.DateTime.Now.TimeOfDay.ToString()},
            {tagResponseGameDay, time.timeTextDay},
            {tagResponseGameYear, time.years.ToString()},
            {tagResponseGameTime, gameTime},
            {tagResponseGameQuestion, questionText.ToString()},
            {tagResponseGameText, responseText}
        };
        previousResponses.Add(newEntry);
        PlayerPrefs.SetString($"{tagResponseIRLDate}{numResponses}", newEntry[tagResponseIRLDate]);
        PlayerPrefs.SetString($"{tagResponseIRLTime}{numResponses}", newEntry[tagResponseIRLTime]);
        PlayerPrefs.SetString($"{tagResponseGameDay}{numResponses}", newEntry[tagResponseGameDay]);
        PlayerPrefs.SetString($"{tagResponseGameYear}{numResponses}", newEntry[tagResponseGameYear]);
        PlayerPrefs.SetString($"{tagResponseGameTime}{numResponses}", newEntry[tagResponseGameTime]);
        PlayerPrefs.SetString($"{tagResponseGameQuestion}{numResponses}", newEntry[tagResponseGameQuestion]);
        PlayerPrefs.SetString($"{tagResponseGameText}{numResponses}", newEntry[tagResponseGameText]);
        numResponses++;
        PlayerPrefs.SetInt("numResponses", numResponses);
        StartCoroutine(StopResponse());
    }

    IEnumerator StopResponse()
    {
        saveResponseText.SetActive(true);
        inputField.GetComponent<TMP_InputField>().enabled = false;
        yield return new WaitForSeconds(1);
        saveResponseText.SetActive(false);
        FindObjectOfType<CapsuleResponseViewer>().HideCapsuleResponse();
    }

    // public void DisplayResponse()
    // {
    //     // Write new entries into left side
    //     string newText = "";
    //     foreach (var entry in previousResponses)
    //     {
    //         // Format: "Year X Day - 00:00: Diary text goes here."
    //         newText += $@"Year {entry[tagGameYear]} {entry[tagGameDay]} - {entry[tagGameTime]}: {entry[tagGameText]}" + "\n\n";
    //     }
    //     prevDiaryText.GetComponent<TMP_Text>().SetText(newText);
    // }
}
