using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapsuleResponseSaver : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    public Button saveResponseButton;
    public TMP_Text questionText;
    public GameObject inputField;
    public GameObject saveResponseText;

    private TaskManager taskManager;
    private CapsuleViewManager capsuleViewManager;
    private TimeController time;

    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        taskManager = FindObjectOfType<TaskManager>();
        capsuleViewManager = FindObjectOfType<CapsuleViewManager>();
        time = FindObjectOfType<TimeController>();
        saveResponseText.SetActive(false);
        // inputField.SetActive(true);
    }

    public void SaveResponse()
    {
        string questionText = this.questionText.text;
        string responseText = inputField.GetComponent<TMP_InputField>().text;

        // Set up separately because these strings are multi-part
        string irlDate = $"{System.DateTime.Now.Month}/{System.DateTime.Now.Day}/{System.DateTime.Now.Year}";
        string gameTime = $"{time.hours.ToString("00")}:{time.mins.ToString("00")}";

        ResponseEntry newEntry = new ResponseEntry();
        newEntry.irlYear = System.DateTime.Now.Year;
        newEntry.irlMonth = System.DateTime.Now.Month;
        newEntry.irlDay = System.DateTime.Now.Day;
        newEntry.irlTime = System.DateTime.Now.TimeOfDay.ToString();
        newEntry.gameYear = time.years;
        newEntry.gameDay = time.timeTextDay;
        newEntry.gameTime = gameTime;
        newEntry.gameQuestion = questionText;
        newEntry.gameText = responseText;

        capsuleViewManager.responses.Add(newEntry);
        capsuleViewManager.numResponses++;
        StartCoroutine(SaveAndCloseResponse());
    }

    IEnumerator SaveAndCloseResponse()
    {
        saveManager.UpdateDataCapsule();
        saveManager.SaveData(false);
        saveResponseText.SetActive(true);
        inputField.GetComponent<TMP_InputField>().enabled = false;
        yield return new WaitForSeconds(1);
        saveResponseText.SetActive(false);
        FindObjectOfType<CapsuleResponseViewer>().HideCapsuleResponse();
    }
}
