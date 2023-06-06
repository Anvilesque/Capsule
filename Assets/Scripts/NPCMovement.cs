using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Yarn.Unity;

public class NPCMovement : MonoBehaviour
{
    public float movementSpeed;
    private TileManager tileManager;
    private Tilemap floorMap;
    private TimeController timeController;
    private NonPC nonPC;
    public List<Sprite> sprites;
    private Vector3Int currentlyFacing;
    private Vector2Int currentTime;
    private Vector3Int currentDestination;
    public List<Vector3Int> directionsToFace;
    public List<Vector2Int> timesHoursMins;
    public List<Vector3Int> destinations;
    public Vector2Int timeToDisappear;
    private int destinationIndex;
    private Vector3Int nearestNodeNPC;
    private Vector3Int nearestNodeDestination;
    public bool isDisappeared;
    private bool isMoving;
    private List<Vector3Int> path;
    private Coroutine movementCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        tileManager = FindObjectOfType<TileManager>();
        timeController = FindObjectOfType<TimeController>();
        floorMap = tileManager.floorMap;
        nonPC = GetComponent<NonPC>();
        CalculateDestinationIndex();
        currentDestination = destinations[destinationIndex];
        nonPC.position = currentDestination;
        isDisappeared = false;
        AttemptDisappear();
        if (isDisappeared) Teleport(false);
        else transform.position = floorMap.CellToWorld(nonPC.position);
        FaceDirection(directionsToFace[destinationIndex]);
        path = FindClosestPath(nonPC.position, currentDestination, tileManager.tilesStandable);
    }

    void FixedUpdate()
    {
        CalculateDestinationIndex();
        AttemptDisappear();
        currentDestination = destinations[destinationIndex];
        if (isMoving) return;
        if (FindObjectOfType<DialogueRunner>().IsDialogueRunning) return;
        if (path.Count == 0)
        {
            if (nonPC.position != currentDestination)
            {
                path = FindClosestPath(nonPC.position, currentDestination, tileManager.tilesStandable);
            }
            else if (currentlyFacing != directionsToFace[destinationIndex])
            {
                FaceDirection(directionsToFace[destinationIndex]);
            }
        }
        else if (path.Count > 0)
        {
            Vector3Int dir = path[0] - nonPC.position;
            if (movementCoroutine == null) movementCoroutine = StartCoroutine(MoveOne(dir));
        }
    }

    void CalculateDestinationIndex()
    {
        destinationIndex = 0;
        foreach (Vector2Int time in timesHoursMins)
        {
            if (timeController.hours >= time.x && timeController.mins >= time.y)
            {
                currentTime = time;
            }
        }
        destinationIndex = timesHoursMins.FindIndex(time => time == currentTime);
    }

    void AttemptDisappear()
    {
        if (timeController.hours >= timeToDisappear.x && timeController.mins >= timeToDisappear.y)
        {
            StartCoroutine(MakeDisappear());
        }
    }

    IEnumerator MakeDisappear()
    {
        float timer = 0f;
        float duration = 3f;
        while (GetComponent<SpriteRenderer>().color.a > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Clamp01(1 - timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        Teleport(false);
    }

    public void Teleport(bool tpToStartingPoint)
    {
        if (tpToStartingPoint)
        {
            isDisappeared = false;
            transform.position = floorMap.CellToWorld(destinations[0]);
        }
        else
        {
            isDisappeared = true;
            transform.position = new Vector3(100f, 100f, 2);
        }
    }

    List<Vector3Int> FindClosestPath(Vector3Int startposition, Vector3Int destination, List<Vector3Int> standableList)
    {
        if (startposition == destination)
            return new List<Vector3Int>();
        Queue<(Vector3Int, List<Vector3Int>)> nodesToSearch = new Queue<(Vector3Int, List<Vector3Int>)>();
        HashSet<Vector3Int> standableSet = new HashSet<Vector3Int>(standableList);
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        nodesToSearch.Enqueue((startposition, new List<Vector3Int>()));
        visited.Add(startposition);
        while (nodesToSearch.Count != 0)
        {
            (Vector3Int pos, List<Vector3Int> path) = nodesToSearch.Dequeue();
            if (pos == destination)
            {
                return path;
            }
            Vector3Int up = pos + Vector3Int.up;
            Vector3Int down = pos + Vector3Int.down;
            Vector3Int left = pos + Vector3Int.left;
            Vector3Int right = pos + Vector3Int.right;
            if (standableSet.Contains(up) && !visited.Contains(up))
            {
                List<Vector3Int> newpath = new List<Vector3Int>(path);
                newpath.Add(up);
                nodesToSearch.Enqueue((up, newpath));
                visited.Add(up);
            }
            if (standableSet.Contains(down) && !visited.Contains(down))
            {
                List<Vector3Int> newpath = new List<Vector3Int>(path);
                newpath.Add(down);
                nodesToSearch.Enqueue((down, newpath));
                visited.Add(down);
            }
            if (standableSet.Contains(left) && !visited.Contains(left))
            {
                List<Vector3Int> newpath = new List<Vector3Int>(path);
                newpath.Add(left);
                nodesToSearch.Enqueue((left, newpath));
                visited.Add(left);
            }
            if (standableSet.Contains(right) && !visited.Contains(right))
            {
                List<Vector3Int> newpath = new List<Vector3Int>(path);
                newpath.Add(right);
                nodesToSearch.Enqueue((right, newpath));
                visited.Add(right);
            }
        }
        // not good if it exists without finding since this means that there is no path!
        return new List<Vector3Int>();
    }
    IEnumerator MoveOne(Vector3Int direction)
    {
        float timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        Vector3Int prevPosition = nonPC.position;
        Vector3Int tempPosition = prevPosition + direction;

        // Check if next position is standable
        if (tileManager.tilesStandable.Contains(tempPosition)) { }
        else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPosition.x, tempPosition.y, tempPosition.z - 2)))
        {
            tempPosition.z -= 2;
        }
        else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPosition.x, tempPosition.y, tempPosition.z + 2)))
        {
            tempPosition.z += 2;
        }
        else yield break;

        // Check if player is in the way
        if (floorMap.WorldToCell(FindObjectOfType<PlayerMovement>().transform.position) == tempPosition) yield break;

        // All checks cleared --> handle movement
        isMoving = true;
        float elapsedTime = 0f;
        path.RemoveAt(0);
        FaceDirection(direction);
        currentlyFacing = direction;
        while (elapsedTime < timeToMove)
        {
            while (FindObjectOfType<DialogueRunner>().IsDialogueRunning) yield return null;
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(floorMap.CellToWorld(prevPosition), floorMap.CellToWorld(tempPosition), (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        nonPC.position = tempPosition;
        transform.position = floorMap.CellToWorld(tempPosition);
        movementCoroutine = null;
        isMoving = false;
    }

    public void FaceDirection(Vector3Int direction)
    {
        const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
        int directionIndex =
            direction == Vector3Int.up ? UP :
            direction == Vector3Int.down ? DOWN :
            direction == Vector3Int.left ? LEFT :
            direction == Vector3Int.right ? RIGHT : -1;
        if (directionIndex == -1) return;
        nonPC.GetComponent<SpriteRenderer>().sprite = sprites[directionIndex];
    }
}
