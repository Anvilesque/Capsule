using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InputFieldManager : MonoBehaviour
{
    public static bool isInputFocused;
    public List<TMP_InputField> inputFields;

    // Start is called before the first frame update
    void Start()
    {
        inputFields = new List<TMP_InputField>(FindObjectsOfType<TMP_InputField>(true));
        isInputFocused = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputFocus();
    }

    void CheckInputFocus()
    {
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
