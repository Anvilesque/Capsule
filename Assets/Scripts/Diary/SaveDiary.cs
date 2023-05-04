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
    public List<string> previousDiaryEntries = new List<string>();
    public GameObject prevDiaryText;

    #region Entry Tags
    public const string tagIRLDate = "irldate";
    public const string tagIRLTime = "irltime";
    public const string tagGameDay = "gametime";
    public const string tagGameTime = "gamedate";
    public const string tagGameText = "gametext";
    #endregion
           
    void Start()
    {
        taskManager = FindObjectOfType<TaskManager>();
        time = FindObjectOfType<TimeController>();
        // Calls Save when button is clicked
        saveDiaryButton.onClick.AddListener(Save);
    }

    public void Save()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        // No text to save --> return
        if (text == "")
            return;
        inputField.GetComponent<TMP_InputField>().text = "";
        taskManager.StopTask();

        // Set up separately because these strings are multi-part
        string irlDate = $"{System.DateTime.Now.Month}/{System.DateTime.Now.Day}/{System.DateTime.Now.Year}";
        string gameTime = $"{time.hours.ToString("00")}:{time.mins.ToString("00")}";
        
        // Tag all data separately, concatenate them into one string
        string rawEntry = TagString(irlDate, tagIRLDate)
                        + TagString(System.DateTime.Now.TimeOfDay.ToString(), tagIRLTime)
                        + TagString(time.days.ToString(), tagGameDay)
                        + TagString(gameTime, tagGameTime)
                        + TagString(text.ToString(), tagGameText);
        previousDiaryEntries.Add(rawEntry);

        // Write new entries into left side
        string newText = "";
        foreach(var entry in previousDiaryEntries)
        {
            // Format: "Day X - 00:00: Diary text goes here."
            newText += $@"Day {GetSubstringByTag(entry, tagGameDay)} - {GetSubstringByTag(entry, tagGameTime)}: {GetSubstringByTag(entry, tagGameText)}" + "\n";
        }
        prevDiaryText.GetComponent<TMP_Text>().SetText(newText);
    }

    private string TagString(string str, string tag)
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
    }
}
