using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    public GameObject aboutTab;

    // Start is called before the first frame update
    void Start()
    {
        aboutTab.SetActive(false);
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
        string saveFilePath = "Assets/MyData.json";
        if (File.Exists(saveFilePath)) File.Delete(saveFilePath);
        LoadGame();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AboutGame()
    {
        aboutTab.SetActive(true);
    }
}
