using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct ResponseEntry
{
    public int irlYear;
    public int irlMonth;
    public int irlDay;
    public string irlTime;
    public int gameYear;
    public string gameDay;
    public string gameTime;
    public string gameQuestion;
    public string gameText;
}

[System.Serializable]
public struct BSItemBuilder
{
    public string objectName;
    public int prevID;
    
    public Vector3 position;

    public string spritePath;
    public bool isSpriteEnabled;
    
    public Vector2 itemSize;
    public bool isEntireSizeFilled;
    public List<Vector2> cellsFilledRelative;
    public List<Vector2> cellsFilledRelativeSpecial;
    public List<Vector2> cellsOccupied;
    public string itemName;
    public int itemSubsize;
    public string itemType;
    public string itemColor;
    public bool isBookshelfed;
    public bool isStacked;
    public List<int> stackedItemsIDs;
    public bool isStackable;
    public int stackCount;
    public bool canSupport;
    // BSItemMovementManager

    public Vector2 colliderOffset;
    public Vector2 colliderSize;
}

public class SaveManager : MonoBehaviour
{
    #region Variables
    GameObject player;
    GameObject gameManager;
    PlayerMovement playerMovement;
    PlayerTransition playerTransition;
    YarnFunctions yarnFunctions;
    TimeController timeController;
    TaskManager taskManager;
    BSBoxRandomManager bsBoxRandomManager;
    BSBoxSortedManager bsBoxSortedManager;
    BSGridManager bsGridManager;
    SaveDiary diary;
    CapsuleViewManager capsuleManager;

    public SaveData myData;
    public static string jsonFilePath = "Assets/MyData.json";
    List<BSItemInfo> itemInfosAll;
    #endregion

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.FindWithTag("GameController");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerTransition = player.GetComponent<PlayerTransition>();
        yarnFunctions = GameObject.FindObjectOfType<YarnFunctions>();

        timeController = gameManager.GetComponent<TimeController>();
        taskManager = gameManager.GetComponent<TaskManager>();

        bsBoxRandomManager = GameObject.FindObjectOfType<BSBoxRandomManager>();
        bsBoxSortedManager = GameObject.FindObjectOfType<BSBoxSortedManager>();
        bsGridManager = GameObject.FindObjectOfType<BSGridManager>();

        diary = GameObject.FindObjectOfType<SaveDiary>();
        capsuleManager = GameObject.FindObjectOfType<CapsuleViewManager>();
        
        LoadData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void LoadData()
    {
        myData = (SaveData)ScriptableObject.CreateInstance<SaveData>();
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            JsonUtility.FromJsonOverwrite(jsonData, myData);
        }
    }

    public void SaveData(bool updateAllFirst = true)
    {
        if (updateAllFirst) UpdateDataAll();
        string jsonData = JsonUtility.ToJson(myData);
        File.WriteAllText(jsonFilePath, jsonData);
    }

    public void UpdateDataAll()
    {
        UpdateDataPlayer();
        UpdateDataTime();
        UpdateDataBookshelf();
        UpdateDataBalance();
        UpdateDataDiary();
        UpdateDataCapsule();
    }

    public void UpdateDataPlayer()
    {
        myData.playerTransformPosition = player.transform.position;
        myData.playerCurrentlyFacing = playerMovement.currentlyFacing;
        myData.playerIsInRoom = playerTransition.isInRoom;
        myData.npcToNumberKeys = new List<string>(yarnFunctions.npcToNumber.Keys);
        myData.npcToNumberValues = new List<int>(yarnFunctions.npcToNumber.Values);
    }

    public void UpdateDataTime()
    {
        myData.seconds = timeController.seconds;
        myData.mins = timeController.mins;
        myData.hours = timeController.hours;
        myData.days = timeController.days;
        myData.years = timeController.years;
        // myData.canUpdateTime = timeController.canUpdateTime;
        myData.isShopClosed = timeController.isShopClosed;
    }

    public void UpdateDataBookshelf()
    {
        ConvertBSItemInfoToBuilderAll();
        List<BSItemInfo> bsSortedStackList = new List<BSItemInfo>(bsBoxSortedManager.itemStack);
        bsSortedStackList.Reverse();
        FillListWithIDS(bsBoxRandomManager.itemStorage, myData.bsRandomStorageIDs);
        FillListWithIDS(bsSortedStackList, myData.bsSortedStackIDs);

        myData.occupiedCellsKeys.Clear();
        myData.occupiedCellsValues.Clear();
        foreach (Vector2Int key in bsGridManager.occupiedCells.Keys)
        {
            myData.occupiedCellsKeys.Add(key);
            BSItemBuilder correspondingBuilder = myData.bsItemsInWorldBuilders.Find(builder => builder.prevID == bsGridManager.occupiedCells[key].itemID);
            myData.occupiedCellsValues.Add(correspondingBuilder.prevID);
        }
        myData.shelfInterval = bsGridManager.shelfInterval;
        myData.snapPreviewEnabled = bsGridManager.snapPreviewEnabled;
    }

    BSItemBuilder ConvertBSItemInfoToBuilderSingle(BSItemInfo item)
    {
        BSItemBuilder newBuilder = new BSItemBuilder();
        newBuilder.objectName = item.gameObject.name;
        newBuilder.prevID = item.itemID;
        newBuilder.position = item.transform.position;

        string spriteName = item.sprite.sprite.name;
        newBuilder.spritePath = $"Sprites/Minigames/Bookshelf/{spriteName.Substring(0, spriteName.IndexOf('_'))}/{spriteName}";
        newBuilder.isSpriteEnabled = item.sprite.enabled;

        newBuilder.itemName = item.itemName;
        newBuilder.itemSize = item.itemSize;
        newBuilder.isEntireSizeFilled = item.isEntireSizeFilled;
        newBuilder.cellsFilledRelative = ConvertListV2IntToV2(item.cellsFilledRelative);
        newBuilder.cellsFilledRelativeSpecial = ConvertListV2IntToV2(item.cellsFilledRelativeSpecial);
        newBuilder.cellsOccupied = ConvertListV2IntToV2(item.cellsOccupied);
        newBuilder.itemSubsize = item.itemSubsize;
        newBuilder.itemType = item.itemType;
        newBuilder.itemColor = item.itemColor;
        newBuilder.isBookshelfed = item.isBookshelfed;
        newBuilder.isStacked = item.isStacked;
        newBuilder.stackedItemsIDs = new List<int>();
        foreach (BSItemInfo stackedItem in item.stackedItems)
        {
            newBuilder.stackedItemsIDs.Add(stackedItem.itemID);
        }
        newBuilder.isStackable = item.isStackable;
        newBuilder.stackCount = item.stackCount;
        newBuilder.canSupport = item.canSupport;
        BoxCollider2D collider = item.GetComponent<BoxCollider2D>();
        newBuilder.colliderOffset = collider.offset;
        newBuilder.colliderSize = collider.size;

        return newBuilder;
    }

    void ConvertBSItemInfoToBuilderAll()
    {
        itemInfosAll = new List<BSItemInfo>(FindObjectsOfType<BSItemInfo>(true));
        myData.bsItemsInWorldBuilders.Clear();
        foreach (BSItemInfo item in itemInfosAll)
        {
            myData.bsItemsInWorldBuilders.Add(ConvertBSItemInfoToBuilderSingle(item));
        }
    }

    List<Vector2> ConvertListV2IntToV2(List<Vector2Int> list)
    {
        List<Vector2> newList = new List<Vector2>();
        foreach (Vector2Int v2Int in list)
        {
            newList.Add(v2Int);
        }
        return newList;
    }

    void FillListWithIDS(List<BSItemInfo> inputList, List<int> outputList)
    {
        outputList.Clear();
        foreach (BSItemInfo item in inputList)
        {
            outputList.Add(item.itemID);
        }
    }

    public void UpdateDataBalance()
    {
        myData.balance = taskManager.balance;
    }

    public void UpdateDataDiary()
    {
        myData.numDiaryEntries = diary.numDiaryEntries;
        myData.diaryEntries = diary.previousEntries;
    }
        
    public void UpdateDataCapsule()
    {
        myData.numCapsuleResponses = capsuleManager.numResponses;
        myData.capsuleResponses = capsuleManager.responses;
    }
}
