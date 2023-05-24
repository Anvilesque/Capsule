using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageCreator : MonoBehaviour
{
    public Vector3 spawnLocation;
    public Vector2 spawnVelocity;
    // Use this for initialization
    void Start()
    {
        StartCoroutine("SpawnObjects");
    }

    IEnumerator SpawnObjects()
    {
        while (true) // a boolean - could just be "true" or could be controlled elsewhere
        {
            spawnLocation = new Vector3(-12,Random.Range(4,6),0);
            GameObject SpawnLocation = (GameObject)Instantiate(Resources.Load("Prefabs/Minigames/Packaging Minigame/Package"), 
            spawnLocation, Quaternion.identity);
            float delay = Random.Range(1f, 5f); // adjust this to set frequency of obstacles
            yield return new WaitForSeconds(delay);
        }
    }

}
