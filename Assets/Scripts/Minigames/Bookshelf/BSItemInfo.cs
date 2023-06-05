using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemInfo : MonoBehaviour
{
    public Vector2Int itemSize;
    [HideInInspector ]public SpriteRenderer sprite;
    [SerializeField] private bool isEntireSizeFilled;
    public List<Vector2Int> cellsFilledRelative {get; private set;}
    public List<Vector2Int> cellsFilledRelativeSpecial;
    public List<Vector2Int> cellsOccupied;
    public string itemName;
    public int itemSubsize;
    public string itemType;
    public string itemColor;
    [HideInInspector] public int itemID;
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
        itemID = Random.Range(0, int.MaxValue);
        cellsFilledRelative = new List<Vector2Int>();
        UpdateCellsFilled();
        cellsOccupied = new List<Vector2Int>();
        if (itemName == "") itemName = $"{itemColor} {itemType}";
        stackCount = 1;
        stackedItems = new Stack<BSItemInfo>();
        stackedItems.Push(this);
        isBookshelfed = false;
        isStacked = false;
    }

    void UpdateCellsFilled()
    {
        cellsFilledRelative.Clear();
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
