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
    List<string> previousDiaryEntries = new List<string>();
           
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
        previousDiaryEntries.Add(text);
        foreach(var entry in previousDiaryEntries)
        {
            Debug.Log(entry);
        }
    }
}
