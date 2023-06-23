using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BSBoxRandomManager : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    public List<BSItemInfo> itemStorage;
    public BSItemInfo currentItem {get; private set;}
    private Camera bookshelfCam;    
    private BSGridManager bookshelfGrid;
    Vector3 mousePos;
    bool pickedUpItem;
    public List<GameObject> itemsDay1, itemsDay2, itemsDay3, itemsDay4, itemsDay5, itemsDay6, itemsDay7;
    public List<List<GameObject>> itemsDayAll;
    public TMP_Text countText;
    [HideInInspector] public List<int> itemsDayAllCount;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        itemStorage = new List<BSItemInfo>();
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfCam = bookshelfGrid.bookshelfCam;
        mousePos = bookshelfGrid.mousePos;

        itemsDayAll = new List<List<GameObject>>() {itemsDay1, itemsDay2, itemsDay3, itemsDay4, itemsDay5, itemsDay6, itemsDay7};
        itemsDayAllCount = new List<int>() {10, 10, 3, 5, 5, 7, 10};
        if (currentItem == null) ChooseNextItem();
        pickedUpItem = false;
        RegisterRandomItems();
    }

    void Update()
    {
        mousePos = bookshelfGrid.mousePos;
        countText.text = $"Items left: {itemStorage.Count}";
        if (itemStorage.Count == 0) return;
        if (Input.GetMouseButtonUp(0))
        {
            if (pickedUpItem) ChooseNextItem();
            pickedUpItem = false;
        }
        if ((GetComponent<Collider2D>().bounds.min.x <= mousePos.x && mousePos.x <= GetComponent<Collider2D>().bounds.max.x
            && GetComponent<Collider2D>().bounds.min.y <= mousePos.y && mousePos.y <= GetComponent<Collider2D>().bounds.max.y))
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentItem.sprite.enabled = true;
                currentItem.transform.position = mousePos;
                // currentItem.isInRandom = false;
                itemStorage.Remove(currentItem);
                pickedUpItem = true;
                return;
            }
            if (currentItem == null) ChooseNextItem();
            else if (pickedUpItem) return;
            else
            {
                currentItem.gameObject.layer = LayerMask.NameToLayer("Default");
                currentItem.transform.position = mousePos;
            }
        }
        else
        {
            if (Input.GetMouseButton(0)) return;
            foreach (BSItemInfo item in itemStorage)
            {
                item.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); 
                item.transform.position = transform.position;
            }
        }
    }

    private void ChooseNextItem()
    {
        if (itemStorage.Count == 0)
        {
            currentItem = null;
            return;   
        }
        currentItem = itemStorage[Random.Range(0, itemStorage.Count - 1)];
    }

    public void AddObjectsTest()
    {
        AddObjects(itemsDay1, 20, false);
    }

    public void AddObjects(List<GameObject> objectsToAdd, int count, bool addExactList = false)
    {
        if (objectsToAdd.Count == 0) return;
        if (count == 0) return;
        if (addExactList)
        {
            foreach (GameObject obj in objectsToAdd)
            {
                GameObject newObject = Instantiate(obj, transform.position, Quaternion.identity);
                itemStorage.Add(newObject.GetComponent<BSItemInfo>());
            }
        }
        else
        {
            int counter = 0;
            while (counter < count)
            {
                GameObject newObject = Instantiate(objectsToAdd[Random.Range(0, objectsToAdd.Count)], transform.position, Quaternion.identity);
                itemStorage.Add(newObject.GetComponent<BSItemInfo>());
                counter++;
            }
        }
        RegisterRandomItems();     
    }

    public void RegisterRandomItems()
    {
        foreach (BSItemInfo item in itemStorage)
        {
            item.isBookshelfed = false;
            // item.isInRandom = true;
            item.GetComponent<SpriteRenderer>().enabled = false;
            item.transform.position = transform.position;
            item.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }   
    }
}
