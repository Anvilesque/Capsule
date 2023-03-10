using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    private RectTransform rectTransform;
    private float width, height;
    [HideInInspector] public Transform parentAfterDrag;
    private Camera gameCam;
    private Canvas canvasBookshelf;
    
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    private float origWidth, origHeight;

    private void Start()
    {
        canvasBookshelf = new List<Canvas>(FindObjectsOfType<Canvas>()).Find(x=> x.name.Contains("Bookshelf"));
        gameCam = new List<Camera>(FindObjectsOfType<Camera>()).Find(x=> x.name.Contains("Bookshelf"));
        rigidBody = GetComponent<Rigidbody2D>();
        origWidth = GetComponent<RectTransform>().rect.width;
        origHeight = GetComponent<RectTransform>().rect.height;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Begin drag.");
        parentAfterDrag = transform.root;
        transform.SetParent(parentAfterDrag);
        transform.SetAsLastSibling();
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(origWidth, origHeight);
        transform.GetComponent<BoxCollider2D>().size = new Vector2(origWidth, origHeight);
        image.raycastTarget = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // Debug.Log("On drag.");
        ChangePos();
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("End drag.");
        ChangePos();
        rigidBody.constraints = RigidbodyConstraints2D.None;
        transform.SetParent(parentAfterDrag);
        if (transform.parent != transform.root)
        {
            GridLayoutGroup grid = transform.parent.gameObject.GetComponent<GridLayoutGroup>();
            if (transform.parent.childCount >= 2)
            {
                Vector2 gridUnitSize = new Vector2(75, 75);
                foreach (BoxCollider2D childCollider in transform.parent.GetComponentsInChildren<BoxCollider2D>())
                {
                    childCollider.size = gridUnitSize;
                }
                if (grid == null)
                {
                    grid = transform.parent.gameObject.AddComponent<GridLayoutGroup>();
                    grid.cellSize = gridUnitSize;
                    grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    grid.constraintCount = 2;
                }
            }
            else
            {
                if (grid != null) Destroy(grid);
                transform.position = transform.parent.position;
            }
            
        }
        image.raycastTarget = true;
    }

    private void ChangePos()
    {
        if (canvasBookshelf == null) return;
        else switch (canvasBookshelf.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            {
                Vector3 mousePosition = Input.mousePosition;
                transform.position = mousePosition;
                break;
            }
            case RenderMode.ScreenSpaceCamera:
            {
                Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, mousePos.y, 150f);
                transform.rotation = Quaternion.identity;
                break;
            }
        }
    }
}
