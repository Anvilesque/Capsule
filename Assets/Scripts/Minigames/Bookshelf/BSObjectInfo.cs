using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSObjectInfo : MonoBehaviour
{
    public Vector2Int size;
    [SerializeField] private bool isEntireSizeFilled;
    public List<Vector2Int> cellsFilled {get; private set;}
    public List<Vector2Int> cellsFilledSpecial;
    [SerializeField] private string type;
    [SerializeField] private string color;
    [HideInInspector] public bool isDisplayed;
    [HideInInspector] public bool isStacked;
    // Start is called before the first frame update
    void Start()
    {
        cellsFilled = new List<Vector2Int>();
        if (isEntireSizeFilled)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                cellsFilled.Add(new Vector2Int(x, y));
            }
        }
        else cellsFilled = cellsFilledSpecial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
