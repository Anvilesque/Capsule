using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Package : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 spawnVelocity = new Vector2(GetRandomNumber(1f,13f, 0.6f), GetRandomNumber(1f,6f));
        GetComponent<Rigidbody2D>().velocity = spawnVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // This is for getting a random number- a probabilityPower >1 biases toward getting smaller numbers
    // while a probabilityPower <1 biases toward getting larger numbers.
    private float GetRandomNumber(float min, float max, float probabilityPower = 1f)
    {
        float randomFloat = UnityEngine.Random.Range(0f,1f);

        float result = (float)(min + (max + 1 - min) * (Math.Pow(randomFloat, probabilityPower)));
        return result;
    }
    private void OnTriggerEnter2D(Collider2D collission)
    {
        if(collission.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }
    }
}
