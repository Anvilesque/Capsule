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
    public List<string> previousDiaryEntriesAlt = new List<string>();
           
    void Start()
    {
        //Calls Save when button is clicked
        saveDiaryButton.onClick.AddListener(Save);
    }

    public void Save()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        inputField.GetComponent<TMP_InputField>().text = "";
        taskManager.StopTask();

        text = $"({System.DateTime.Now.Month}/{System.DateTime.Now.Day}/{System.DateTime.Now.Year})Day 1- {time.hours.ToString("00")}:{time.mins.ToString("00")}: {text}"; 
        previousDiaryEntries.Add(text);
        foreach(var entry in previousDiaryEntries)
        {
            Debug.Log(entry);
        }
    }
}
