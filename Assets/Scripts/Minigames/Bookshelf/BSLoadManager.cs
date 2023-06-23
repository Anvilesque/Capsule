using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSLoadManager : MonoBehaviour
{
    private string jsonFilePath = SaveManager.jsonFilePath;
    private List<BSItemInfo> itemInfosAll;
    private BSBoxRandomManager bsBoxRandomManager;
    private BSBoxSortedManager bsBoxSortedManager;
    private BSGridManager bsGridManager;
    private SaveData myData;

    void Start()
    {
        myData = FindObjectOfType<SaveManager>().myData;
        bsBoxRandomManager = FindObjectOfType<BSBoxRandomManager>();
        bsBoxSortedManager = FindObjectOfType<BSBoxSortedManager>();
        bsGridManager = FindObjectOfType<BSGridManager>();

        LoadBSData();
    }

    void LoadBSData()
    {
        ConvertBSBuilderToObjects();
        FillBSContainers();
        UpdateIDs();
    }

    void ConvertBSBuilderToObjects()
    {
        itemInfosAll = new List<BSItemInfo>();
        foreach (BSItemBuilder itemBuilder in myData.bsItemsInWorldBuilders)
        {
            System.Type[] componentTypes = new System.Type[]
            {
                typeof(SpriteRenderer),
                typeof(BSItemInfo),
                typeof(BSItemMovementManager),
                typeof(BoxCollider2D)
            };
            GameObject itemObject = new GameObject(itemBuilder.objectName, componentTypes);
            itemObject.transform.SetParent(GameObject.FindWithTag("Bookshelf").transform);

            Transform itemTransform = itemObject.GetComponent<Transform>();
            SpriteRenderer itemSprite = itemObject.GetComponent<SpriteRenderer>();
            BSItemInfo itemInfo = itemObject.GetComponent<BSItemInfo>();
            itemInfosAll.Add(itemInfo);
            BSItemMovementManager itemMovementManager = itemObject.GetComponent<BSItemMovementManager>();
            BoxCollider2D itemCollider = itemObject.GetComponent<BoxCollider2D>();
            
            itemTransform.position = itemBuilder.position;

            itemSprite.sprite = Resources.Load<Sprite>(itemBuilder.spritePath);
            itemSprite.enabled = itemBuilder.isSpriteEnabled;

            itemInfo.itemID = itemBuilder.prevID;
            itemInfo.itemSize = Vector2Int.RoundToInt(itemBuilder.itemSize);
            itemInfo.sprite = itemSprite;
            itemInfo.isEntireSizeFilled = itemBuilder.isEntireSizeFilled;
            itemInfo.cellsFilledRelativeSpecial = new List<Vector2Int>();
            foreach (Vector2 cell in itemBuilder.cellsFilledRelativeSpecial)
            {
                itemInfo.cellsFilledRelativeSpecial.Add(Vector2Int.RoundToInt(cell));
            }
            itemInfo.UpdateCellsFilled();
            itemInfo.cellsOccupied = new List<Vector2Int>();
            foreach (Vector2 cell in itemBuilder.cellsOccupied)
            {
                itemInfo.cellsOccupied.Add(Vector2Int.RoundToInt(cell));
            }
            itemInfo.itemName = itemBuilder.itemName;
            itemInfo.itemSubsize = itemBuilder.itemSubsize;
            itemInfo.itemType = itemBuilder.itemType;
            itemInfo.itemColor = itemBuilder.itemColor;
            itemInfo.isBookshelfed = itemBuilder.isBookshelfed;
            itemInfo.isStacked = itemBuilder.isStacked;
            itemInfo.stackedItems = new Stack<BSItemInfo>();
            
            itemInfo.isStackable = itemBuilder.isStackable;
            itemInfo.stackCount = itemBuilder.stackCount;
            itemInfo.canSupport = itemBuilder.canSupport;

            itemCollider.offset = itemBuilder.colliderOffset;
            itemCollider.size = itemBuilder.colliderSize;
        }
        // Fill in stackedItems for each BSItemInfo
        foreach (BSItemInfo itemInfo in itemInfosAll)
        {
            BSItemBuilder correspondingBuilder = myData.bsItemsInWorldBuilders.Find(builder => builder.prevID == itemInfo.itemID);
            foreach (int ID in correspondingBuilder.stackedItemsIDs)
            {
                itemInfo.stackedItems.Push(itemInfosAll.Find(item => item.itemID == ID));
            }
        }
    }    

    void FillBSContainers()
    {
        bsBoxRandomManager.itemStorage.Clear();
        bsBoxSortedManager.itemStack.Clear();
        bsGridManager.occupiedCells.Clear();
        foreach (int ID in myData.bsRandomStorageIDs)
        {
            bsBoxRandomManager.itemStorage.Add(itemInfosAll.Find(item => item.itemID == ID));
        }
        foreach (int ID in myData.bsSortedStackIDs)
        {
            bsBoxSortedManager.itemStack.Push(itemInfosAll.Find(item => item.itemID == ID));
        }
        for (int keyNum = 0; keyNum < myData.occupiedCellsKeys.Count; keyNum++)
        {
            Vector2Int nextKey = Vector2Int.RoundToInt(myData.occupiedCellsKeys[keyNum]);
            BSItemInfo nextValue = itemInfosAll.Find(item => item.itemID == myData.occupiedCellsValues[keyNum]);
            bsGridManager.occupiedCells.Add(nextKey, nextValue);
        }
    }

    void UpdateIDs()
    {
        foreach (BSItemInfo itemInfo in itemInfosAll)
        {
            itemInfo.itemID = itemInfo.gameObject.GetInstanceID();
        }
    }
}
