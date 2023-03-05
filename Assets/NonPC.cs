using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Yarn.Unity;

public class NonPC : MonoBehaviour
{
    public string introTitle;
    public Vector3 position {get; private set;}
    private TileManager tileManager;
    private Tilemap floorMap;
    private Tilemap wallMap;
    private Tilemap transitionMap;
    private Tilemap interactableMap;
    private DialogueRunner dialogueRunner;

    private const float RAND_MVMT_MIN = 3f;
    private const float RAND_MVMT_MAX = 10f;
    private float randMvmtTimer;
    private float randMvmtCooldown;
    private bool isMoving;
    private IEnumerator randMvmtCoroutine;

    // private float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        wallMap = tileManager.wallMap;
        transitionMap = tileManager.transitionMap;
        interactableMap = tileManager.interactableMap;
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        // movementSpeed = 3f;
        ResetRandMvmt();
        
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        if (!isMoving && !dialogueRunner.IsDialogueRunning)
        {
            randMvmtTimer -= Time.deltaTime;
        }
        if (randMvmtTimer <= 0)
        {
            // Move();
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
        Vector3Int destination = tileManager.tilesStandable[Random.Range(0, tileManager.tilesStandable.Count)];
        destination.z = 2;
        transform.position = TileManager.GridCoordsToWorldCoords(destination) + new Vector3(0, 2 * TileManager.distY, 0); // set to teleport for now
        isMoving = false;
        
        // TODO: Implement A* pathfinding algorithm
    }
}
