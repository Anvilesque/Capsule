using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoBack : MonoBehaviour
{
    private Button goBackButton;
    // Start is called before the first frame update
    void Start()
    {
        goBackButton = GetComponent<Button>();
        goBackButton.onClick.AddListener(Back);
    }
    void Back()
    {
        SceneManager.LoadScene("Scenes/Showcase");
    }
}
