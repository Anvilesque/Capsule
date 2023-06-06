using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InputFieldManager : MonoBehaviour
{
    List<TMP_InputField> inputFields;
    
    public static bool isInputFocused {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        isInputFocused = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputFocus();
    }

    void CheckInputFocus()
    {
        inputFields = new List<TMP_InputField>(FindObjectsOfType<TMP_InputField>());
        foreach (TMP_InputField field in inputFields)
        {
            if (field.isFocused)
            {
                isInputFocused = true;
                return;
            }
        }
        isInputFocused = false;
    }
}
