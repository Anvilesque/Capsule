using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveData", menuName = "SaveData/SaveData")]
public class SaveData : ScriptableObject
{
    // === PLAYER ===
    public Vector3 playerTransformPosition = new Vector3(-12.5f, -10.75f, 2);
    public Vector3Int playerCurrentlyFacing = Vector3Int.left;
    // public Vector3Int currentPos;
    // public bool canMove;

    // public bool canTeleport;
    public bool playerIsInRoom = true;
    // === NPC ===
    public List<string> npcToNumberKeys = new List<string>();
    public List<int> npcToNumberValues = new List<int>();
    // potentially change it so that NPC's store exact position when quitting, i.e., mid-walk
    // this would also mean saving NPC's path

    // === TIME ===
    public float seconds = 0;
    public int mins = 0;
    public int hours = 8;
    public int days = 0;
    public int years = 1;

    // public bool canUpdateTime = true;
    public bool isShopClosed = false;

    // === BOOKSHELF ===
    public List<BSItemBuilder> bsItemsInWorldBuilders = new List<BSItemBuilder>();
    public List<int> bsRandomStorageIDs = new List<int>();
    public List<int> bsSortedStackIDs = new List<int>();

    public List<Vector2> occupiedCellsKeys = new List<Vector2>();
    public List<int> occupiedCellsValues = new List<int>();
    public int shelfInterval = 4;
    public bool snapPreviewEnabled = false;
    
    // Figure out how to save BSItemInfo and its transforms
    // Figure out how to save BSItemMovementManager values

    // === PACKAGING ===
    public int balance = 0;

    // === CLEANING ===
    // Probably, eventually, how many left to clean, their progress, etc.

    // === DIARY ===
    public int numDiaryEntries = 0;
    public List<ResponseEntry> diaryEntries = new List<ResponseEntry>();

    // === CAPSULE ===
    public int numCapsuleResponses = 0;
    public List<ResponseEntry> capsuleResponses = new List<ResponseEntry>();
    // potentially saving entry last left off, like currentResponse
}
