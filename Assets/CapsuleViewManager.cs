using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CapsuleViewManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text responseText;
    public int currentResponseIndex;
    List<Dictionary<string, string>> previousResponses;

    // Start is called before the first frame update
    void Start()
    {
        previousResponses = FindObjectOfType<CapsuleResponseSaver>().previousResponses;
        ResetCapsuleView();
        // foreach (var entry in previousDiaryEntries)
        // {
        //     // Format: "Year X Day - 00:00: Diary text goes here."
        //     newText += $@"Year {entry[tagGameYear]} {entry[tagGameDay]} - {entry[tagGameTime]}: {entry[tagGameText]}" + "\n\n";
        // // }
    }

    public void ResetCapsuleView()
    {
        currentResponseIndex = 0;
        SetCapsuleView();
    }

    public void SetCapsuleView()
    {
        Dictionary<string, string> currentResponse;
        if (previousResponses.Count == 0) return;
        currentResponse = previousResponses[currentResponseIndex];
        questionText.SetText($@"{currentResponse[CapsuleResponseSaver.tagResponseGameQuestion]} (Year {currentResponse[CapsuleResponseSaver.tagResponseGameYear]} {currentResponse[CapsuleResponseSaver.tagResponseGameDay]} at {currentResponse[CapsuleResponseSaver.tagResponseGameTime]})");
        responseText.SetText(currentResponse[CapsuleResponseSaver.tagResponseGameText]);
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
        if (currentResponseIndex >= previousResponses.Count) currentResponseIndex -= 1;
        else SetCapsuleView();
    }
}
