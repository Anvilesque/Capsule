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
    private VisualElement tasksRoot;
    public GameObject menu;
    public GameObject settings;
    public GameObject tasks;
    private UIDocument buttonDocument;
    public Button menuButton;
    public Button settingsButton;
    public Button tasksButton;

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

        // menu = GameObject.Find("HUD");
        // if (menu != null)
        // {
        //     Debug.Log("Menu found");
        // }

        menuRoot = menu.GetComponent<UIDocument>().rootVisualElement;
        // if (menuRoot != null)
        // {
        //     Debug.Log("Menu UI document found");
        // }

        // settings = GameObject.Find("SettingsHUD");
        // if (settings != null)
        // {
        //     Debug.Log("Settings found");
        // }


        settingsRoot = settings.GetComponent<UIDocument>().rootVisualElement;

        // if (settingsRoot != null)
        // {
        //     Debug.Log("Setting UI document found");
        // }

        // tasks = GameObject.Find("TaskMap");
        // if (tasks != null)
        // {
        //     Debug.Log("tasks found");
        // }

        tasksRoot = tasks.GetComponent<UIDocument>().rootVisualElement;
        // if (tasksRoot != null)
        // {
        //     Debug.Log("tasks UI document found");
        // }

        tasksRoot.style.display = DisplayStyle.None;
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
        // if (settingsButton != null)
        // {
        //     Debug.Log("Setting button found");
        // }
        // else
        // {
        //     Debug.LogError("Setting button not found");
        // }

        settingsButton.RegisterCallback<ClickEvent>(SettingsButtonClick);

        tasksButton = buttonDocument.rootVisualElement.Q("map-button") as Button;
        // if (tasksButton != null)
        // {
        //     Debug.Log("Task button found");
        // }
        // else
        // {
        //     Debug.LogError("Task button not found");
        // }

        tasksButton.RegisterCallback<ClickEvent>(TasksButtonClick);
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

    void TasksButtonClick(ClickEvent evt)
    {
        if (tasksRoot.style.display == DisplayStyle.Flex) 
        {
            tasksRoot.style.display = DisplayStyle.None;
        }
        else 
        {
            tasksRoot.style.display = DisplayStyle.Flex;
        }
    }

    public void EnableHUD()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void DisableHUD()
    {
        root.style.display = DisplayStyle.None;
    }
}
