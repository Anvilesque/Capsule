using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapsuleResponseViewer : MonoBehaviour
{
    public static bool isWriting {get; private set;}

    Canvas capsuleResponseCanvas;
    TMP_Text questionText;
    Image npcIcon;
    public List<Sprite> sprites;
    Dictionary<string, int> npcNameToSpriteIndex;

    // Start is called before the first frame update
    void Start()
    {
        capsuleResponseCanvas = new List<Canvas>(FindObjectsOfType<Canvas>()).Find(canvas => canvas.name.Contains("Response"));
        questionText = new List<TMP_Text>(FindObjectsOfType<TMP_Text>()).Find(text => text.name.Contains("QuestionView"));
        npcIcon = new List<Image>(FindObjectsOfType<Image>()).Find(image => image.name.Contains("NPC"));
        npcNameToSpriteIndex = new Dictionary<string, int>()
        {
            {"ben", 0},
            {"aldrich", 1},
            {"joanne", 2},
            {"winter", 3}
        };
        capsuleResponseCanvas.gameObject.SetActive(false);
        isWriting = false;
    }

    public void DisplayCapsuleResponse(string npcName, string question)
    {
        isWriting = true;
        npcIcon.sprite = sprites[npcNameToSpriteIndex[npcName]];
        questionText.SetText(question);
        capsuleResponseCanvas.GetComponentInChildren<TMP_InputField>().text = "";
        capsuleResponseCanvas.gameObject.SetActive(true);
        FindObjectOfType<HUDButtons>().ToggleHUD();
    }

    public void HideCapsuleResponse()
    {
        isWriting = false;
        capsuleResponseCanvas.gameObject.SetActive(false);
        FindObjectOfType<HUDButtons>().ToggleHUD();
    }
}
