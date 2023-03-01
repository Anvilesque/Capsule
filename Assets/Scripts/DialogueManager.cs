using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    private bool dialogueRunning;
    public void StartDialogueRunner()
    {
        dialogueRunning=true;
        dialogueRunner.StartDialogue("Intro");
    }
    public bool Dialog()
    {
        return dialogueRunning;
    }
}
