using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemInfo : MonoBehaviour
{
    [HideInInspector] public int itemID;   
    public Vector2Int itemSize;
    [HideInInspector] public SpriteRenderer sprite;

    public bool isEntireSizeFilled;
    public List<Vector2Int> cellsFilledRelative {get; private set;}
    public List<Vector2Int> cellsFilledRelativeSpecial;
    public List<Vector2Int> cellsOccupied;

    public string itemName;
    public int itemSubsize;
    public string itemType;
    public string itemColor;

    [HideInInspector] public bool isBookshelfed;
    [HideInInspector] public bool isStacked;
    public Stack<BSItemInfo> stackedItems;

    public bool isStackable;
    public int stackCount;
    public bool canSupport;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        itemID = GetInstanceID();
        UpdateCellsFilled();
        cellsOccupied = new List<Vector2Int>();
        if (itemName == "") itemName = $"{itemColor} {itemType}";
        stackCount = 1;
        stackedItems = new Stack<BSItemInfo>();
        stackedItems.Push(this);
        isBookshelfed = false;
        isStacked = false;
        // isInRandom = false;
    }

    public void UpdateCellsFilled()
    {
        cellsFilledRelative = new List<Vector2Int>();
        if (isEntireSizeFilled)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                cellsFilledRelative.Add(new Vector2Int(x, y));
            }
        }
        else cellsFilledRelative = cellsFilledRelativeSpecial;
    }
}
