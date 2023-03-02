using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPC : MonoBehaviour
{
    public string introTitle;
    public Vector3 position {get; private set;}

    private const float RAND_MVMT_MIN = 3f;
    private const float RAND_MVMT_MAX = 10f;
    private float randMvmtTimer;
    private float randMvmtCooldown;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        ResetRandMvmt();
        
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        if (!isMoving)
        {
            randMvmtTimer -= Time.deltaTime;
        }
        if (randMvmtTimer <= 0)
        {
            Move();
            ResetRandMvmt();
        }
    }

    void ResetRandMvmt()
    {
        randMvmtCooldown = Random.Range(RAND_MVMT_MIN, RAND_MVMT_MAX);
        randMvmtTimer = randMvmtCooldown;
    }

    void Move()
    {
        isMoving = true;
        
    }
}
