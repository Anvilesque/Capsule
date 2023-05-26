using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDButtons : MonoBehaviour
{
    UIDocument buttonDocument;
    Button settingButton;

    void OnEnable()
    {
        buttonDocument = GetComponent<UIDocument>();
        if (buttonDocument == null)
        {
            Debug.LogError("No button document found.");
        }

        settingButton = buttonDocument.rootVisualElement.Q("settings-button") as Button;
        settingButton.RegisterCallback<ClickEvent>(SettingButtonClick);
    }

    public void SettingButtonClick(ClickEvent evt)
    {

    }

}
