using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CapsuleViewManager : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    public TMP_Text questionText;
    public TMP_Text responseText;
    public int currentResponseIndex;
    [HideInInspector] public int numResponses;
    public List<ResponseEntry> responses;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        numResponses = saveData.numCapsuleResponses;
        responses = saveData.capsuleResponses;
        ResetCapsuleView();
    }

    public void ResetCapsuleView()
    {
        currentResponseIndex = 0;
        SetCapsuleView();
    }

    public void SetCapsuleView()
    {
        ResponseEntry currentResponse;
        if (responses.Count == 0) return;
        currentResponse = responses[currentResponseIndex];
        // Question goes here? (Year # Day_name at 00:00)
        questionText.SetText($"{currentResponse.gameQuestion.ToString()} (Year {currentResponse.gameYear} {currentResponse.gameDay} at {currentResponse.gameTime})");
        responseText.SetText(currentResponse.gameText);
    }

    public void GoToPreviousView()
    {
        currentResponseIndex -= 1;
        if (currentResponseIndex < 0) currentResponseIndex = 0;
        else SetCapsuleView();
    }

    public void GoToNextView()
    {
        currentResponseIndex += 1;
        if (currentResponseIndex >= responses.Count) currentResponseIndex = responses.Count - 1;
        else SetCapsuleView();
    }
}
