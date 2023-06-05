using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class HUDButtons : MonoBehaviour
{
    private VisualElement root;
    private VisualElement menuRoot;
    private GameObject menu;
    public UIDocument buttonDocument;
    public Button menuButton;


    private Label timeTime;
    private Label timeDay;
    private TimeController timeController;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        timeController = FindObjectOfType<TimeController>();
        timeTime = root.Q<Label>("timeTime");
        Debug.Log(timeTime);
        timeDay = root.Q<Label>("timeDay");
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;

        menu = GameObject.Find("HUD");
        if (menu != null)
        {
            Debug.Log("Menu found");
        }

        menuRoot = menu.GetComponent<UIDocument>().rootVisualElement;
        if (menuRoot != null)
        {
            Debug.Log("Menu UI document found");
        }
    }

    void OnEnable() 
    {
        buttonDocument = GetComponent<UIDocument>();

        if (buttonDocument == null)
        {
            Debug.LogError("No button document found.");
        }
        else 
        {
            Debug.Log("Button document found.");
        }

        menuButton = buttonDocument.rootVisualElement.Q("menu-button") as Button;

        if (menuButton != null)
        {
            Debug.Log("Button found");
        }
        else
        {
            Debug.LogError("Button not found");
        }

        menuButton.RegisterCallback<ClickEvent>(MenuButtonClick);
    }

    private void Update()
    {
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;  
    }

    void MenuButtonClick(ClickEvent evt)
    {
        Debug.Log("HI");

        if (menuRoot.style.display == DisplayStyle.Flex) 
        {
            menuRoot.style.display = DisplayStyle.None;
        }
        else 
        {
            menuRoot.style.display = DisplayStyle.Flex;
        }
    }

}
