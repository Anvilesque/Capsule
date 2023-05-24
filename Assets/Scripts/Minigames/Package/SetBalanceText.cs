using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SetBalanceText : MonoBehaviour
{
    private int balance;
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        balance = PlayerPrefs.GetInt("money", 0);
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        balance = PlayerPrefs.GetInt("money", 0);
        text.SetText($"Balance: {balance}");
    }
}
