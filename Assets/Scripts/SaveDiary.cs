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
    public TaskManager taskManager;
    public TimeController time;
    public List<string> previousDiaryEntries = new List<string>();
    public GameObject prevDiaryText;
           
    void Start()
    {
        //Calls Save when button is clicked
        saveDiaryButton.onClick.AddListener(Save);
    }

    public void Save()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        // no text to save, so return
        if (text == "")
            return;
        inputField.GetComponent<TMP_InputField>().text = "";
        taskManager.StopTask();

        //$ is for putting in variables, @ is for breaking into multiple lines
        text = $@"{System.DateTime.Now.Month}/{System.DateTime.Now.Day}/{System.DateTime.Now.Year}
        |Day 1- {time.hours.ToString("00")}:{time.mins.ToString("00")}: {text}";
        previousDiaryEntries.Add(text);
        //write new entries into left side.
        string newText = "";
        foreach(var entry in previousDiaryEntries)
        {
            //only take the part after | for now
            string[] parts = entry.Split('|');
            newText += $"\n {parts[1]}";
        }
        prevDiaryText.GetComponent<TMP_Text>().SetText(newText);
    }
}
