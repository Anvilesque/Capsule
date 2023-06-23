using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveDiary : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    public Button saveDiaryButton;
    public GameObject inputField;
    private TaskManager taskManager;
    private TimeController time;
    public List<ResponseEntry> previousEntries;
    public GameObject prevDiaryText;
    public GameObject saveDiaryText;
    public int numDiaryEntries;

    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        numDiaryEntries = saveData.numDiaryEntries;
        previousEntries = saveData.diaryEntries;
        taskManager = FindObjectOfType<TaskManager>();
        time = FindObjectOfType<TimeController>();
        // Calls Save when button is clicked
        saveDiaryButton.onClick.AddListener(Save);
        saveDiaryText.SetActive(false);
        inputField.SetActive(true);
        UpdateDiaryText();
    }

    public void Save()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        // No text to save --> return
        if (text == "")
            return;
        inputField.GetComponent<TMP_InputField>().text = "";

        // Set up separately because these strings are multi-part
        string gameTime = $"{time.hours.ToString("00")}:{time.mins.ToString("00")}";

        // Tag all data separately, concatenate them into one string
        ResponseEntry newEntry = new ResponseEntry();
        newEntry.irlYear = System.DateTime.Now.Year;
        newEntry.irlMonth = System.DateTime.Now.Month;
        newEntry.irlDay = System.DateTime.Now.Day;
        newEntry.irlTime = System.DateTime.Now.TimeOfDay.ToString();
        newEntry.gameYear = time.years;
        newEntry.gameDay = time.timeTextDay;
        newEntry.gameTime = gameTime;
        newEntry.gameText = text.ToString();
        previousEntries.Add(newEntry);
        numDiaryEntries++;
        UpdateDiaryText();
        StartCoroutine(SaveAndCloseDiary());
    }

    void UpdateDiaryText()
    {
        string newText = "";
        foreach (ResponseEntry entry in previousEntries)
        {
            // "Year X Day - 00:00: Diary text goes here."
            newText += $"Year {entry.gameYear} {entry.gameDay} - {entry.gameTime}: {entry.gameText}" + "\n\n";
        }
        prevDiaryText.GetComponent<TMP_Text>().SetText(newText);
    }

    IEnumerator SaveAndCloseDiary()
    {
        saveManager.UpdateDataDiary();
        saveManager.SaveData(false);
        saveDiaryText.SetActive(true);
        inputField.GetComponent<TMP_InputField>().enabled = false;
        yield return new WaitForSeconds(1);
        saveDiaryText.SetActive(false);
        taskManager.StopTask();
    }
}
