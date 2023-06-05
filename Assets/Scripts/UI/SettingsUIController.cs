using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class SettingsUIController : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    private const float ROOT_WIDTH_PERCENTAGE = 0.2f;
    private float rootWidth;
    private Button buttonSound;
    private Button buttonSupport;
    private Button buttonQuit;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        buttonSound = root.Q<Button>("buttonSound");
        buttonSupport = root.Q<Button>("buttonSupport");
        buttonQuit = root.Q<Button>("buttonQuit");

        buttonSound.clicked += buttonSoundPressed;
        buttonSupport.clicked += buttonSupportPressed;
        buttonQuit.clicked += buttonQuitPressed;

        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        rootWidth = (ROOT_WIDTH_PERCENTAGE * 1080f) * ((float)Screen.width / (float)Screen.height);
    }

    void buttonSoundPressed()
    {

    }

    void buttonSupportPressed()
    {

    }

    void buttonQuitPressed() 
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}