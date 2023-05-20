using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSItemInfo : MonoBehaviour
{
    public Vector2Int itemSize;
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
    public bool isStackable;
    public int stackCount;
    // Start is called before the first frame update
    void Start()
    {
        itemID = Random.Range(0, int.MaxValue);
        cellsFilled = new List<Vector2Int>();
        UpdateCellsFilled();
        if (itemName == "") itemName = $"{itemColor} {itemType}";
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
