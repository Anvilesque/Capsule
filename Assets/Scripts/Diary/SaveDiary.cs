using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveDiary : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button saveDiaryButton;
    public GameObject inputField;
    private TaskManager taskManager;
    private TimeController time;
    public List<Dictionary<string, string>> previousDiaryEntries = new List<Dictionary<string, string>>();
    public GameObject prevDiaryText;
    public GameObject saveDiaryText;
    public int numDiaryEntries;

    #region Entry Tags
    public const string tagIRLDate = "irldate";
    public const string tagIRLTime = "irltime";
    public const string tagGameDay = "gametime";
    public const string tagGameTime = "gamedate";
    public const string tagGameText = "gametext";
    #endregion

    void Start()
    {
        numDiaryEntries = PlayerPrefs.GetInt("numDiaryEntries", 0);
        for (int i = 0; i < numDiaryEntries; i++)
        {
            previousDiaryEntries.Add(new Dictionary<string, string>(){
                { tagIRLDate, PlayerPrefs.GetString($"{tagIRLDate}{i}") },
                { tagIRLTime, PlayerPrefs.GetString($"{tagIRLTime}{i}") },
                { tagGameDay, PlayerPrefs.GetString($"{tagGameDay}{i}") },
                { tagGameTime, PlayerPrefs.GetString($"{tagGameTime}{i}") },
                { tagGameText, PlayerPrefs.GetString($"{tagGameText}{i}") },
            });
        }
        taskManager = FindObjectOfType<TaskManager>();
        time = FindObjectOfType<TimeController>();
        // Calls Save when button is clicked
        saveDiaryButton.onClick.AddListener(Save);
        saveDiaryText.SetActive(false);
        inputField.SetActive(true);
        string newText = "";
        foreach (var entry in previousDiaryEntries)
        {
            // Format: "Day X - 00:00: Diary text goes here."
            newText += $@"Day {entry[tagGameDay]} - {entry[tagGameTime]}: {entry[tagGameText]}" + "\n";
        }
        prevDiaryText.GetComponent<TMP_Text>().SetText(newText);
    }

    public void Save()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        // No text to save --> return
        if (text == "")
            return;
        inputField.GetComponent<TMP_InputField>().text = "";

        // Set up separately because these strings are multi-part
        string irlDate = $"{System.DateTime.Now.Month}/{System.DateTime.Now.Day}/{System.DateTime.Now.Year}";
        string gameTime = $"{time.hours.ToString("00")}:{time.mins.ToString("00")}";

        // Tag all data separately, concatenate them into one string
        Dictionary<string, string> newEntry = new Dictionary<string, string>(){
            {tagIRLDate, irlDate},
            {tagIRLTime, System.DateTime.Now.TimeOfDay.ToString()},
            {tagGameDay, time.days.ToString()},
            {tagGameTime, gameTime},
            {tagGameText, text.ToString()}
        };
        previousDiaryEntries.Add(newEntry);
        PlayerPrefs.SetString($"{tagIRLDate}{numDiaryEntries}", newEntry[tagIRLDate]);
        PlayerPrefs.SetString($"{tagIRLTime}{numDiaryEntries}", newEntry[tagIRLTime]);
        PlayerPrefs.SetString($"{tagGameDay}{numDiaryEntries}", newEntry[tagGameDay]);
        PlayerPrefs.SetString($"{tagGameTime}{numDiaryEntries}", newEntry[tagGameTime]);
        PlayerPrefs.SetString($"{tagGameText}{numDiaryEntries}", newEntry[tagGameText]);
        numDiaryEntries++;
        PlayerPrefs.SetInt("numDiaryEntries", numDiaryEntries);

        // Write new entries into left side
        string newText = "";
        foreach (var entry in previousDiaryEntries)
        {
            // Format: "Day X - 00:00: Diary text goes here."
            newText += $@"Day {entry[tagGameDay]} - {entry[tagGameTime]}: {entry[tagGameText]}" + "\n";
        }
        prevDiaryText.GetComponent<TMP_Text>().SetText(newText);
        StartCoroutine(StopDiary());
    }
    IEnumerator StopDiary()
    {
        saveDiaryText.SetActive(true);
        inputField.GetComponent<TMP_InputField>().enabled = false;
        yield return new WaitForSeconds(1);
        saveDiaryText.SetActive(false);
        taskManager.StopTask();
    }

    /* private string TagString(string str, string tag)
    {
        // <tag>str</tag>
        return $"<{tag}>{str}</{tag}>";
    }

    private string GetSubstringByTag(string str, string tag)
    {
        // Start at the end of the start-tag (i.e., the character right after the '>')
        int startingPoint = str.IndexOf($"<{tag}>") + $"<{tag}>".Length;
        // End at the start of the end-tag (i.e., the character right before the '</')
        return str.Substring(startingPoint, str.IndexOf($"</{tag}>") - startingPoint);
    } */
}
