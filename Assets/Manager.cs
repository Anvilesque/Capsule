using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> antiques;
    GameObject currAntique;
    int currAntiqueIndex;
    
    public TMP_Text statusMessage;
    int numToClean;
    int numCleaned;
    bool jobDone;

    // Start is called before the first frame update
    void Start()
    {
        jobDone = false;
        numToClean = Random.Range(3, 6);
        statusMessage.text = "Antiques left: " + numToClean;
        numCleaned = 0;
        currAntiqueIndex = Random.Range(0, antiques.Count);
        currAntique = antiques[currAntiqueIndex];
        currAntique.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!currAntique.activeSelf && !jobDone)  // try to check for complete status 
        {
            numCleaned++;
            statusMessage.text = "Antiques left: " + (numToClean - numCleaned);

            if (numCleaned == numToClean) // end minigame, get paycheck?!
            {
                jobDone = true;
                Debug.Log("job done :)");
                return;
            }

            currAntiqueIndex = Random.Range(0, antiques.Count);
            currAntique = antiques[currAntiqueIndex];
            currAntique.SetActive(true);
        } else
        {
            // Debug.Log("antique " + currAntiqueIndex + " is not active");
        }
    }
}

