using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    public GameObject aboutTab;
    bool aboutBuffer;

    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs.DeleteAll();
        aboutTab.SetActive(false);
        aboutBuffer = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            aboutTab.SetActive(false);
        }
    }

    public void StartGame()
    {
        PlayerPrefs.DeleteAll();
        LoadGame();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AboutGame()
    {
        aboutBuffer = true;
        aboutTab.SetActive(true);
    }
}
