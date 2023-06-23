using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;


public class BSEvaluationManager : MonoBehaviour
{
    BSGridManager bookshelfGrid;
    Tilemap bookshelfMap;
    List<BSItemInfo> items;
    public TMP_Text evaluationText;
    private int countItems;
    private int countItemsTotal;
    private int countTypes;
    private float percentStacked;
    private float percentSymmetryX;
    private float percentSymmetryY;
    private float percentSymmetry;
    private float percentAdjacentType;
    private float percentAdjacentColor;
    private float percentBestSubsize;

    // Start is called before the first frame update
    void Start()
    {
        bookshelfGrid = FindObjectOfType<BSGridManager>();
        bookshelfMap = bookshelfGrid.GetComponent<Tilemap>();
        bookshelfMap.CompressBounds();
        evaluationText.gameObject.SetActive(false);
    }

    public void ResetEvaluation()
    {
        items = new List<BSItemInfo>(FindObjectsOfType<BSItemInfo>());
        countItems = 0;
        countItemsTotal = 0;
        countTypes = 0;
        percentAdjacentType = 0;
        percentAdjacentColor = 0;
        percentBestSubsize = 0;
        percentSymmetry = 0;
        percentSymmetryX = 0;
        percentSymmetryY = 0;
        percentStacked = 0;
    }

    public void DisplayEvaluation()
    {
        ResetEvaluation();
        EvaluateNumberOfItems();
        EvaluateType();
        EvaluateColor();
        EvaluateSubsize();
        EvaluateSymmetry();
        EvaluateStacked();
        float scoreAbundance = countItems / (float)countItemsTotal * 0.8f;
        float scoreAesthetic = Mathf.Clamp01(Mathf.Max(percentAdjacentType, percentAdjacentColor) + percentBestSubsize * 0.1f);
        float scoreOrganization = Mathf.Clamp01(percentSymmetry + percentStacked * 0.05f);
        float score = Mathf.Max(scoreAbundance, scoreAesthetic, scoreOrganization);
        string rating;
        rating = score >= 1 ? "S" :
                score >= 0.8 ? "A" :
                score >= 0.4 ? "B" :
                                "C";
        evaluationText.text = $"Great job! This is {rating}-level work!";
        evaluationText.gameObject.SetActive(true);
        StartCoroutine(FadeUpErrorText(evaluationText));
        Debug.Log("Total score: " + Mathf.Max(scoreAbundance, scoreAesthetic, scoreOrganization));
    }

    IEnumerator FadeUpErrorText(TMP_Text evaluationText)
    {
        float holdTimer = 0f;
        float holdDuration = 0.5f;
        float fadeTimer = 0f;
        float fadeDuration = 0.5f;
        float moveUpInterval = 0.01f;
        Vector3 originalPosition = evaluationText.rectTransform.position;
        while (holdTimer < holdDuration)
        {
            evaluationText.rectTransform.position = new Vector3(evaluationText.rectTransform.position.x, evaluationText.rectTransform.position.y + moveUpInterval, evaluationText.rectTransform.position.z);
            holdTimer += Time.deltaTime;
            yield return null;
        }
        while (evaluationText.color.a > 0)
        {
            evaluationText.rectTransform.position = new Vector3(evaluationText.rectTransform.position.x, evaluationText.rectTransform.position.y + moveUpInterval, evaluationText.rectTransform.position.z);
            evaluationText.color = new Color(evaluationText.color.r, evaluationText.color.g, evaluationText.color.b, 1 - fadeTimer / fadeDuration);
            fadeTimer += Time.deltaTime;
            yield return null;
        }
        evaluationText.gameObject.SetActive(false);
        evaluationText.rectTransform.position = originalPosition;
        evaluationText.color = new Color(evaluationText.color.r, evaluationText.color.g, evaluationText.color.b, 1);
    }

    public void EvaluateNumberOfItems()
    {
        countItems = 0;
        countItemsTotal = 0;
        foreach (BSItemInfo item in items)
        {
            countItemsTotal++;
            if (item.isBookshelfed)
            {
                countItems++;
            }
        }
    }

    public void EvaluateNumberOfTypes()
    {
        Dictionary<string, int> typesToCount = new Dictionary<string, int>();
        foreach (BSItemInfo item in items)
        {
            typesToCount[item.itemType] += 1;
        }
        countTypes = typesToCount.Count;
    }

    public void EvaluateType()
    {
        Dictionary<string, int> typeToCountTotal = new Dictionary<string, int>();
        Dictionary<string, int> typeToCountAdjacent = new Dictionary<string, int>();
        int sameAdjacentTypeCount = 0;
        int sameAdjacentTypeTotal = 0;;
        // Find how many of each type there are
        foreach (BSItemInfo item in items)
        {
            if (!typeToCountTotal.ContainsKey(item.itemType))
            {
                typeToCountTotal[item.itemType] = 0;
                typeToCountAdjacent[item.itemType] = 0;
            }
            typeToCountTotal[item.itemType]++;
        }
        // Find how many items are adjacent to at least one item of the same type
        foreach (BSItemInfo item in items)
        {
            List<Vector2Int> adjacentCells = FindAdjacentCells(item);
            bool isAdjacent = false;
            foreach (Vector2Int adjacentCell in adjacentCells)
            {
                if (!bookshelfGrid.occupiedCells.ContainsKey(adjacentCell)) continue;
                if (bookshelfGrid.occupiedCells[adjacentCell].itemType == item.itemType)
                {
                    isAdjacent = true;
                    break;
                }
            }
            if (isAdjacent) typeToCountAdjacent[item.itemType]++;
        }
        foreach (string type in typeToCountTotal.Keys)
        {
            sameAdjacentTypeCount += typeToCountAdjacent[type];
            if (typeToCountTotal[type] > 1)
            sameAdjacentTypeTotal += typeToCountTotal[type];
        }
        percentAdjacentType = sameAdjacentTypeCount / (float)sameAdjacentTypeTotal;
    }

    public void EvaluateColor()
    {
        Dictionary<string, int> colorToCountTotal = new Dictionary<string, int>();
        Dictionary<string, int> colorToCountAdjacent = new Dictionary<string, int>();
        int sameAdjacentColorCount = 0;
        int sameAdjacentColorTotal = 0;
        // Find how many of each color there are
        foreach (BSItemInfo item in items)
        {
            if (!colorToCountTotal.ContainsKey(item.itemColor))
            {
                colorToCountTotal[item.itemColor] = 0;
                colorToCountAdjacent[item.itemColor] = 0;
            }
            colorToCountTotal[item.itemColor]++;
        }
        // Find how many items are adjacent to at least one item of the same color
        foreach (BSItemInfo item in items)
        {
            List<Vector2Int> adjacentCells = FindAdjacentCells(item);
            bool isAdjacent = false;
            foreach (Vector2Int adjacentCell in adjacentCells)
            {
                if (!bookshelfGrid.occupiedCells.ContainsKey(adjacentCell)) continue;
                if (bookshelfGrid.occupiedCells[adjacentCell].itemColor == item.itemColor)
                {
                    isAdjacent = true;
                    break;
                }
            }
            if (isAdjacent) colorToCountAdjacent[item.itemColor]++;
        }
        foreach (string color in colorToCountTotal.Keys)
        {
            sameAdjacentColorCount += colorToCountAdjacent[color];
            if (colorToCountTotal[color] > 1)
            sameAdjacentColorTotal += colorToCountTotal[color];
        }
        percentAdjacentColor = sameAdjacentColorCount / (float)sameAdjacentColorTotal;
    }

    public void EvaluateSubsize()
    {
        HashSet<int> itemSubsizes = new HashSet<int>();
        HashSet<Vector2Int> itemBottomRightCells = new HashSet<Vector2Int>();
        foreach (BSItemInfo item in items)
        {
            if (item.itemSubsize == 0) continue;
            itemBottomRightCells.Add(FindBottomRightCell(item));            
            itemSubsizes.Add(item.itemSubsize);
        }
        
        int maxStreak = 1;
        int maxStreakPossible = itemSubsizes.Count;
        foreach (Vector2Int itemBottomRightPosition in itemBottomRightCells)
        {
            Vector2Int currentCell = itemBottomRightPosition;
            Vector2Int leftCell = currentCell + Vector2Int.left;
            // Find the leftmost item in this chain
            while (bookshelfMap.cellBounds.Contains((Vector3Int)(leftCell)))
            {
                if (itemBottomRightCells.Contains(leftCell)) currentCell = leftCell;
                leftCell = leftCell + Vector2Int.left;
            }

            int streak = 1;
            bool isIncreasing = false;
            bool isDecreasing = false;
            Vector2Int rightCell = currentCell + Vector2Int.right;
            while (bookshelfMap.cellBounds.Contains((Vector3Int)(rightCell)))
            {
                if (!bookshelfGrid.occupiedCells.ContainsKey(currentCell) || !bookshelfGrid.occupiedCells.ContainsKey(rightCell)) {}
                else if (bookshelfGrid.occupiedCells[currentCell].itemSubsize == bookshelfGrid.occupiedCells[rightCell].itemSubsize + 1)
                {
                    if (!isDecreasing)
                    {
                        if (!isIncreasing)
                        {
                            isDecreasing = true;
                            streak++;
                        }
                        else break;
                    }
                    else streak++;
                }
                else if (bookshelfGrid.occupiedCells[currentCell].itemSubsize == bookshelfGrid.occupiedCells[rightCell].itemSubsize - 1)
                {
                    if (!isIncreasing)
                    {
                        if (!isDecreasing)
                        {
                            isIncreasing = true;
                            streak++;
                        }
                        else break;
                    }
                    else streak++;
                }
                currentCell = rightCell;
                rightCell = currentCell + Vector2Int.right;
            }
            maxStreak = Mathf.Max(streak, maxStreak);
        }
        if (maxStreakPossible == 0) percentBestSubsize = 1f;
        else percentBestSubsize = maxStreak / (float)maxStreakPossible;
    }

    private List<Vector2Int> FindAdjacentCells(BSItemInfo item)
    {
        HashSet<Vector2Int> adjacentCells = new HashSet<Vector2Int>();
        foreach (Vector2Int cell in item.cellsOccupied)
        {
            foreach (Vector2Int direction in TileManager.cardinalDirections)
            {
                if (item.cellsOccupied.Contains(cell + direction)) continue;
                adjacentCells.Add(cell + direction);
            }
        }
        return new List<Vector2Int>(adjacentCells);
    }

    private Vector2Int FindBottomRightCell(BSItemInfo item)
    {
        Vector2Int bottomRightCell = new Vector2Int(int.MinValue, int.MaxValue);
        foreach (Vector2Int cell in item.cellsOccupied)
        {
            if (cell.x >= bottomRightCell.x && cell.y <= bottomRightCell.y) bottomRightCell = cell;
        }
        return bottomRightCell;
    }

    public void EvaluateSymmetry()
    {
        int maxSymmetryCountX = 0;
        int maxSymmetryCountY = 0;
        int totalCountX = 0;
        int totalCountY = 0;
        List<Vector2Int> allPositions = new List<Vector2Int>();
        foreach (Vector2Int cellPosition in bookshelfGrid.occupiedCells.Keys)
        {
            totalCountX++;
            totalCountY++;
        }
        totalCountX /= 2;
        totalCountY /= 2;
        foreach (Vector2Int originPosition in bookshelfMap.cellBounds.allPositionsWithin)
        {
            int tempSymmetryCountX = 0;
            int tempSymmetryCountY = 0;
            foreach (Vector2Int cellPosition in bookshelfGrid.occupiedCells.Keys)
            {
                if (cellPosition.x <= originPosition.x)
                {
                    Vector2Int oppositeXCell = new Vector2Int(originPosition.x + (originPosition.x - cellPosition.x) + 1, cellPosition.y);
                    if (!bookshelfGrid.occupiedCells.ContainsKey(oppositeXCell)) {}
                    else if (bookshelfGrid.occupiedCells[cellPosition].itemType == bookshelfGrid.occupiedCells[oppositeXCell].itemType
                    || bookshelfGrid.occupiedCells[cellPosition].itemColor == bookshelfGrid.occupiedCells[oppositeXCell].itemColor)
                    {
                        tempSymmetryCountX++;
                    }
                }
                if (cellPosition.y <= originPosition.y)
                {
                    Vector2Int oppositeYCell = new Vector2Int(cellPosition.x, originPosition.y + (originPosition.y - cellPosition.y) + 1);
                    if (!bookshelfGrid.occupiedCells.ContainsKey(oppositeYCell)) {}
                    else if (bookshelfGrid.occupiedCells[cellPosition].itemType == bookshelfGrid.occupiedCells[oppositeYCell].itemType
                    || bookshelfGrid.occupiedCells[cellPosition].itemColor == bookshelfGrid.occupiedCells[oppositeYCell].itemColor)
                    {
                        tempSymmetryCountY++;
                    }
                }
            }
            maxSymmetryCountX = Mathf.Max(tempSymmetryCountX, maxSymmetryCountX);
            maxSymmetryCountY = Mathf.Max(tempSymmetryCountY, maxSymmetryCountY);
        }
        percentSymmetryX = Mathf.Clamp01(maxSymmetryCountX / (float)totalCountX);
        percentSymmetryY = Mathf.Clamp01(maxSymmetryCountY / (float)totalCountY);
        percentSymmetry = Mathf.Max(percentSymmetryX, percentSymmetryY);
    }

    public void EvaluateStacked()
    {
        int totalStackable = 0;
        int totalStacked = 0;
        foreach (BSItemInfo item in items)
        {
            if (item.isStackable)
            {
                totalStackable++;
                if (item.isStacked) totalStacked++;
            }
        }
        percentStacked = totalStacked / (float)totalStackable;
    }
}
