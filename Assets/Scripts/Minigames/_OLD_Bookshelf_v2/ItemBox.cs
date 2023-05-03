using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemBox : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Camera gameCam;
    private List<Object> items;
    private bool clicked;
    
    // Start is called before the first frame update
    void Start()
    {
        gameCam = new List<Camera>(FindObjectsOfType<Camera>()).Find(x=> x.name.Contains("Bookshelf"));
        items = new List<Object>();
        foreach (GameObject item in Resources.LoadAll("Prefabs/Minigames/_OLD_Bookshelf_v2/Items"))
        {
            items.Add(item);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clicked) return;
        clicked = true;
        ItemExplosion(3);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
    }

    private void ItemExplosion(int numItems)
    {
        Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 150f;
        for (int i = 0; i < numItems; i++)
        {
            GameObject newItem;
            float lampChance = 1f;
            if (Random.Range(0f, 100f) < lampChance)
            {
                newItem = (GameObject)Instantiate(items[items.Count - 1], mousePos, Quaternion.identity, transform.root.Find("Items"));
            }
            else
            {
                newItem = (GameObject)Instantiate(items[Random.Range(0, items.Count - 1)], mousePos, Quaternion.identity, transform.root.Find("Items"));
            }
            newItem.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            newItem.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        }
    }
}
