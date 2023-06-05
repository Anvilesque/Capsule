using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class HUDButtons : MonoBehaviour
{
    private VisualElement root;
    private VisualElement menuRoot;
    private VisualElement settingsRoot;
    private GameObject menu;
    private GameObject settings;

    public UIDocument buttonDocument;
    public Button menuButton;
    public Button settingsButton;

    private Label timeTime;
    private Label timeDay;
    private TimeController timeController;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.Flex;
        timeController = FindObjectOfType<TimeController>();
        timeTime = root.Q<Label>("timeTime");
        timeDay = root.Q<Label>("timeDay");
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;

        menu = GameObject.Find("HUD");
        // if (menu != null)
        // {
        //     Debug.Log("Menu found");
        // }

        menuRoot = menu.GetComponent<UIDocument>().rootVisualElement;
        // if (menuRoot != null)
        // {
        //     Debug.Log("Menu UI document found");
        // }

        settings = GameObject.Find("SettingsHUD");
        // if (settings != null)
        // {
        //     Debug.Log("Settings found");
        // }

        // settingsRoot = settings.GetComponent<UIDocument>().rootVisualElement;
        // if (settingsRoot != null)
        // {
        //     Debug.Log("Setting UI document found");
        // }
    }


    void OnEnable() 
    {
        buttonDocument = GetComponent<UIDocument>();

        // if (buttonDocument == null)
        // {
        //     Debug.LogError("No button document found.");
        // }
        // else 
        // {
        //     Debug.Log("Button document found.");
        // }

        menuButton = buttonDocument.rootVisualElement.Q("menu-button") as Button;

        // if (menuButton != null)
        // {
        //     Debug.Log("Button found");
        // }
        // else
        // {
        //     Debug.LogError("Button not found");
        // }

        menuButton.RegisterCallback<ClickEvent>(MenuButtonClick);

        settingsButton = buttonDocument.rootVisualElement.Q("settings-button") as Button;
        if (settingsButton != null)
        {
            Debug.Log("Setting button found");
        }
        else
        {
            Debug.LogError("Setting button not found");
        }

        settingsButton.RegisterCallback<ClickEvent>(SettingsButtonClick);
    }

    private void Update()
    {
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;  
    }

    void MenuButtonClick(ClickEvent evt)
    {
        menuRoot.style.display = menuRoot.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
    }

    void SettingsButtonClick(ClickEvent evt)
    {
        settingsRoot.style.display = settingsRoot.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void ToggleHUD()
    {
        root.style.display = root.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
