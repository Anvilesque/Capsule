using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public bool isRunningDialogue {get; private set;}
    public void StartDialogueRunner(string dialogueTitle)
    {
        isRunningDialogue = true;
        dialogueRunner.StartDialogue(dialogueTitle);
    }
}
