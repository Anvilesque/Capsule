using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CleaningManager : MonoBehaviour
{
    private List<AntiqueObject> antiques;
    private AntiqueObject currAntique;
    private int currAntiqueIndex;
    
    public TMP_Text statusMessage;
    public TMP_Text completeMessage;
    private int numToClean;
    private int numCleaned;
    private bool jobDone;

    // Start is called before the first frame update
    void Start()
    {
        antiques = new List<AntiqueObject>(FindObjectsOfType<AntiqueObject>(true));
        ResetMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        if (jobDone)
        {
            if (!completeMessage.gameObject.activeSelf)
            {
                completeMessage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (currAntique.gameObject.activeSelf) return;
            
            numCleaned++;
            statusMessage.text = "Antiques left: " + (numToClean - numCleaned);

            if (numCleaned == numToClean) // end minigame, get paycheck?!
            {
                jobDone = true;
                return;
            }

            GenerateNextAntique();
        }
    }

    void GenerateNextAntique()
    {
        foreach (AntiqueObject antique in antiques)
        {
            if (antique.gameObject.activeSelf) antique.ResetAntique();
        }
        currAntiqueIndex = Random.Range(0, antiques.Count);
        currAntique = antiques[currAntiqueIndex];
        currAntique.gameObject.SetActive(true);
    }

    public void ResetMinigame()
    {
        jobDone = false;
        completeMessage.gameObject.SetActive(false);
        foreach (ToolSlot tool in FindObjectsOfType<ToolSlot>(true))
        {
            tool.DeselectTool();
        }
        numCleaned = 0;
        numToClean = Random.Range(3, 6);
        statusMessage.text = "Antiques left: " + numToClean;
        GenerateNextAntique();
    }
}

