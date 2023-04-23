using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageCreator : MonoBehaviour
{
    public Vector3 spawnLocation;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("SpawnObjects");
    }

    IEnumerator SpawnObjects()
    {
        while (true) // a boolean - could just be "true" or could be controlled elsewhere
        {
            spawnLocation = new Vector3(Random.Range(-9.5f, 9.5f),7,0);
            GameObject SpawnLocation = (GameObject)Instantiate(Resources.Load("Prefabs/Package"), spawnLocation, Quaternion.identity);
            float delay = Random.Range(1f, 4f); // adjust this to set frequency of obstacles
            yield return new WaitForSeconds(delay);
        }
    }

}
