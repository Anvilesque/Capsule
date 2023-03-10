using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnFunctions : MonoBehaviour
{
    public List<string> nodesBen;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<DialogueRunner>().AddFunction("GetRandomDialogue", (int numDialogue) => {return GetRandomDialogue(numDialogue);} );
    }

    public int GetRandomDialogue(int numDialogue)
    {
        return Random.Range(0, numDialogue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
