using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemInfo : MonoBehaviour
{
    public Vector2Int itemSize;
    public SpriteRenderer sprite;
    [SerializeField] private bool isEntireSizeFilled;
    public List<Vector2Int> cellsFilled {get; private set;}
    public List<Vector2Int> cellsFilledSpecial;
    public string itemName;
    public string itemSubsize;
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
        cellsFilled = new List<Vector2Int>();
        UpdateCellsFilled();
        if (itemName == "") itemName = $"{itemColor} {itemType}";
        stackCount = 1;
        stackedItems = new Stack<BSItemInfo>();
        stackedItems.Push(this);
        isBookshelfed = false;
        isStacked = false;
    }

    void UpdateCellsFilled()
    {
        cellsFilled.Clear();
        if (isEntireSizeFilled)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                cellsFilled.Add(new Vector2Int(x, y));
            }
        }
        else cellsFilled = cellsFilledSpecial;
    }
}
