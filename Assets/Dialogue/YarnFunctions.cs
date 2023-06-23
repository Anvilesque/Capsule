using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnFunctions : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    public List<string> nodesBen;
    private TimeController timeController;
    private DialogueRunner dialogueRunner;
    public Dictionary<string, int> npcToNumber;
    private string[] npcs = new string[] {"ben", "aldrich", "winter", "joanne"};

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        timeController = FindObjectOfType<TimeController>();
        dialogueRunner = GetComponent<DialogueRunner>();
        npcToNumber = new Dictionary<string, int>();
        if (saveData.npcToNumberKeys.Count == 0)
        {
            ResetNPCDialogueNumber();
        }
        else for (int i = 0; i < saveData.npcToNumberKeys.Count; i++)
        {
            npcToNumber.Add(saveData.npcToNumberKeys[i], saveData.npcToNumberValues[i]);
        }
        
        dialogueRunner.AddFunction("GetRandomDialogue", (int numDialogue) => { return GetRandomDialogue(numDialogue); } );
        dialogueRunner.AddFunction("GetDay", () => {return GetDay();} );
        dialogueRunner.AddFunction("GetNPCDialogueNumber", (string npc) => { return GetNPCDialogueNumber(npc); } );
    }
    
    public void ResetNPCDialogueNumber()
    {
        foreach (string npc in npcs)
        {
            if (!npcToNumber.ContainsKey(npc)) npcToNumber.Add(npc, 1);
            else npcToNumber[npc] = 1;
        }
        saveManager.UpdateDataPlayer();
        saveManager.SaveData(false);
    }

    public int GetRandomDialogue(int numDialogue)
    {
        return Random.Range(0, numDialogue);
    }

    public int GetDay()
    {
        return timeController.days;
    }

    public int GetNPCDialogueNumber(string npc)
    {
        return npcToNumber[npc];
    }

    [YarnCommand("setNextDialogueFor")]
    public void IncreaseNPCDialogueNumber(string npc)
    {
        npcToNumber[npc]++;
        saveManager.UpdateDataPlayer();
        saveManager.SaveData(false);
    }

    [YarnCommand("openCapsuleResponse")]
    public void OpenCapsuleResponse(string npc, string question)
    {
        FindObjectOfType<CapsuleResponseViewer>().DisplayCapsuleResponse(npc, question);
    }
}
