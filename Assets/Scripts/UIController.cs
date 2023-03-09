using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public Label timeTime;
    public Label timeDay;
    public Button buttonMenu;
    public Button buttonTask;
    public Button buttonInventory;
    public Button buttonMood;
    public Button buttonSettings;
    public UIDocument tasklist;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        buttonMenu = root.Q<Button>("buttonMenu");
        buttonTask = root.Q<Button>("buttonTask");
        buttonInventory = root.Q<Button>("buttonInventory");
        buttonMood = root.Q<Button>("buttonMood");
        buttonSettings = root.Q<Button>("buttonSettings");

        buttonMenu.clicked += buttonMenuPressed;
        buttonTask.clicked += buttonTaskPressed;
        buttonInventory.clicked += buttonInventoryPressed;
        buttonMood.clicked += buttonMoodPressed;
        buttonSettings.clicked += buttonSettingsPressed;
    }

    void buttonMenuPressed() {

    }

    void buttonTaskPressed() {
        
    }

    void buttonInventoryPressed() {

    }

    void buttonMoodPressed() {

    }

    void buttonSettingsPressed() {

    }
}
