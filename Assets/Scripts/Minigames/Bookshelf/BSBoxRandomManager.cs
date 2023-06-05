using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSBoxRandomManager : MonoBehaviour
{
    List<BSItemInfo> itemStorage;
    BSItemInfo currentItem;
    private Camera bookshelfCam;
    private BSGridManager bookshelfGrid;
    Vector3 mousePos;
    public List<GameObject> itemsDay1, itemsDay2, itemsDay3, itemsDay4, itemsDay5, itemsDay6, itemsDay7;
    public List<List<GameObject>> itemsDayAll;
    [HideInInspector] public List<int> itemsDayAllCount;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage = new List<BSItemInfo>();
        ChooseNextItem();
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfCam = bookshelfGrid.bookshelfCam;
        mousePos = bookshelfGrid.mousePos;

        itemsDayAll = new List<List<GameObject>>() {itemsDay1, itemsDay2, itemsDay3, itemsDay4, itemsDay5, itemsDay6, itemsDay7};
        itemsDayAllCount = new List<int>() {30, 30, 10, 15, 10, 20, 30};
    }

    void Update()
    {
        mousePos = bookshelfGrid.mousePos;
        if (itemStorage.Count == 0) return;
        if ((GetComponent<Collider2D>().bounds.min.x <= mousePos.x && mousePos.x <= GetComponent<Collider2D>().bounds.max.x
            && GetComponent<Collider2D>().bounds.min.y <= mousePos.y && mousePos.y <= GetComponent<Collider2D>().bounds.max.y))
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentItem.sprite.enabled = true;
                currentItem.transform.position = mousePos;
                itemStorage.Remove(currentItem);
                ChooseNextItem();
                return;
            }
            if (currentItem == null) ChooseNextItem();
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
        foreach (BSItemInfo item in itemStorage)
        {
            item.isBookshelfed = false;
            item.GetComponent<SpriteRenderer>().enabled = false;
            item.transform.position = transform.position;
            item.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }        
    }
}
