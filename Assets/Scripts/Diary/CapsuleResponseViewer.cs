using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapsuleResponseViewer : MonoBehaviour
{
    public static bool isWriting {get; private set;}

    public Canvas capsuleResponseCanvas;
    public TMP_Text questionText;
    public Image npcIcon;
    public List<Sprite> sprites;
    private Dictionary<string, int> npcNameToSpriteIndex = new Dictionary<string, int>()
    {
        {"ben", 0},
        {"aldrich", 1},
        {"joanne", 2},
        {"winter", 3}
    };
    private TimeController timeController;

    // Start is called before the first frame update
    void Start()
    {
        timeController = FindObjectOfType<TimeController>();
        capsuleResponseCanvas.gameObject.SetActive(false);
        isWriting = false;
    }

    public void DisplayCapsuleResponse(string npcName, string question)
    {
        isWriting = true;
        timeController.canUpdateTime = false;
        FindObjectOfType<HUDButtons>().DisableHUD();
        npcIcon.sprite = sprites[npcNameToSpriteIndex[npcName]];
        questionText.SetText(question);
        capsuleResponseCanvas.GetComponentInChildren<TMP_InputField>().enabled = true;
        capsuleResponseCanvas.GetComponentInChildren<TMP_InputField>().text = "";
        capsuleResponseCanvas.gameObject.SetActive(true);
    }

    public void HideCapsuleResponse()
    {
        isWriting = false;
        timeController.canUpdateTime = true;
        capsuleResponseCanvas.gameObject.SetActive(false);
        FindObjectOfType<HUDButtons>().EnableHUD();
        FindObjectOfType<PlayerMovement>().EnableMovement();
    }
}
