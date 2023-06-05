using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSBoxSortedManager : MonoBehaviour
{
    Stack<BSItemInfo> itemStack;
    BSItemInfo itemNext;
    Vector3 mousePos;
    
    // Start is called before the first frame update
    void Start()
    {
        itemStack = new Stack<BSItemInfo>();
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        if ((GetComponent<Collider2D>().bounds.min.x <= mousePos.x && mousePos.x <= GetComponent<Collider2D>().bounds.max.x
            && GetComponent<Collider2D>().bounds.min.y <= mousePos.y && mousePos.y <= GetComponent<Collider2D>().bounds.max.y))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (itemStack.TryPop(out itemNext))
                {
                    itemNext.sprite.enabled = true;
                    itemNext.transform.position = mousePos;
                }
                return;
            }
            if (itemStack.TryPeek(out itemNext))
            {
                itemNext.gameObject.layer = LayerMask.NameToLayer("Default");
                itemNext.transform.position = mousePos;
            }
        }
        else
        {
            if (Input.GetMouseButton(0)) return;
            foreach (BSItemInfo item in itemStack)
            {
                item.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); 
                item.transform.position = transform.position;
            }
        }
    }

    public void AddToBox(BSItemInfo item)
    {
        item.isBookshelfed = false;
        item.sprite.enabled = false;
        itemStack.Push(item);
        item.transform.position = transform.position;
        item.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void ClearAllInBox()
    {
        foreach (BSItemInfo item in itemStack)
        {
            Destroy(item.gameObject);
        }
        itemStack.Clear();
    }
}
