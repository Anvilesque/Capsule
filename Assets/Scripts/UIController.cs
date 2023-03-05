using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button menuButton;
    public Button taskButton;
    public Button inventoryButton;
    public Button moodButton;
    public Button settingsButton;
    public UIDocument taskList;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        menuButton = root.Q<Button>("MenuButton");
        taskButton = root.Q<Button>("TaskButton");
        inventoryButton = root.Q<Button>("InventoryButton");
        moodButton = root.Q<Button>("MoodButton");
        settingsButton = root.Q<Button>("SettingsButton");

        menuButton.clicked += MenuButtonPressed;
        taskButton.clicked += TaskButtonPressed;
        inventoryButton.clicked += InventoryButtonPressed;
        moodButton.clicked += MoodButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
    }

    void MenuButtonPressed() {

    }

    void TaskButtonPressed() {
        
    }

    void InventoryButtonPressed() {

    }

    void MoodButtonPressed() {

    }

    void SettingsButtonPressed() {

    }
}
