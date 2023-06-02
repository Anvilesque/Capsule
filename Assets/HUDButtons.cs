using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class HUDButtons : MonoBehaviour
{
    private VisualElement root;
    UIDocument buttonDocument;
    Button settingButton;

    private Label timeTime;
    private Label timeDay;
    private TimeController timeController;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        timeController = FindObjectOfType<TimeController>();
        timeTime = root.Q<Label>("timeTime");
        timeDay = root.Q<Label>("timeDay");
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;
    }

    private void Update()
    {
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;
    }

    void OnEnable()
    {
        buttonDocument = GetComponent<UIDocument>();
<<<<<<< Updated upstream
        if (buttonDocument == null)
        {
            Debug.LogError("No button document found.");
        }

=======
>>>>>>> Stashed changes
        settingButton = buttonDocument.rootVisualElement.Q("settings-button") as Button;
        settingButton.RegisterCallback<ClickEvent>(SettingButtonClick);
    }

    public void SettingButtonClick(ClickEvent evt)
    {

    }

}
