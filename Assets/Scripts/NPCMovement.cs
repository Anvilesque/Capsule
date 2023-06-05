using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMovement : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap floorMap;
    private TimeController timeController;
    private NonPC nonPC;
    private Vector3Int destination;
    public List<Vector3Int> destinations;
    private int destinationInd;
    private Vector3Int nearestNodeNPC;
    private Vector3Int nearestNodeDestination;
    private bool isMoving;
    private List<Vector3Int> path;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        tileManager = FindObjectOfType<TileManager>();
        timeController = FindObjectOfType<TimeController>();
        floorMap = tileManager.floorMap;
        nonPC = GetComponent<NonPC>();
        path = FindClosestPath(nonPC.position, destination, tileManager.tilesStandable);
        destinationInd = 0;
    }

    void FixedUpdate()
    {
        switch(timeController.hours)
        {
            case > 18:
                destinationInd = 3;
                break;
            case > 12:
                destinationInd = 2;
                break;
            case > 6:
                destinationInd = 1;
                break;
            default:
                destinationInd = 0;
                break;
        }
        destination = destinations[destinationInd];
        if(isMoving)
            return;
        if (path.Count == 0 && nonPC.position != destination)
        {
            path = FindClosestPath(nonPC.position, destination, tileManager.tilesStandable);
        }
        else if (path.Count > 0)
        {
            Vector3Int dir = path[0] - nonPC.position;
            StartCoroutine(MoveOne(dir));
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
        float movementSpeed = 4f;
        float timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        FaceDirection(direction);

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
        while (elapsedTime < timeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(floorMap.CellToWorld(prevPosition), floorMap.CellToWorld(tempPosition), (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = floorMap.CellToWorld(tempPosition);
        isMoving = false;
    }

    void FaceDirection(Vector3Int direction)
    {
        const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
        int directionIndex =
            direction == Vector3Int.up ? UP :
            direction == Vector3Int.down ? DOWN :
            direction == Vector3Int.left ? LEFT :
            direction == Vector3Int.right ? RIGHT : -1;
        if (directionIndex == -1) return;
        // nonPC.sprite = sprites[directionIndex];
    }
}
