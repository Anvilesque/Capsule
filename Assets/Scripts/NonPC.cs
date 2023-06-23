using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Yarn.Unity;

public class NonPC : MonoBehaviour
{
    public string introTitle;
    public Vector3Int position;
    private TileManager tileManager;
    private Tilemap floorMap;
    private NPCMovement npcMovement;

    // private float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        position = floorMap.WorldToCell(transform.position);
        npcMovement = GetComponent<NPCMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!npcMovement.isDisappeared)
        position = floorMap.WorldToCell(transform.position);
    }
}
