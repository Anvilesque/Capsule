using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnFunctions : MonoBehaviour
{
    public List<string> nodesBen;
    private TimeController timeController;
    private DialogueRunner dialogueRunner;
    private Dictionary<string, int> npcToNumber;
    private List<string> npcs;

    // Start is called before the first frame update
    void Start()
    {
        timeController = FindObjectOfType<TimeController>();
        dialogueRunner = GetComponent<DialogueRunner>();
        
        npcs = new List<string>() {"ben", "aldrich", "winter", "joanne"};
        npcToNumber = new Dictionary<string, int>();
        ResetNPCDialogueNumber();

        dialogueRunner.AddFunction("GetRandomDialogue", (int numDialogue) => {return GetRandomDialogue(numDialogue);} );
        dialogueRunner.AddFunction("GetDay", () => {return GetDay();} );
        dialogueRunner.AddFunction("GetNPCDialogueNumber", (string npc) => {return GetNPCDialogueNumber(npc);} );
    }
    
    public void ResetNPCDialogueNumber()
    {
        foreach (string npc in npcs)
        {
            npcToNumber[npc] = 1;
        }
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
    }
}
