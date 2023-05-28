using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMovement : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap floorMap;
    private NonPC nonPC;
    private Vector3Int destination;
    private Vector3Int nearestNodeNPC;
    private Vector3Int nearestNodeDestination;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        nonPC = GetComponent<NonPC>();
    }

    void Update()
    {

    }

    public void MoveNPCToDest(Vector3Int destination)
    {
        nearestNodeNPC = FindNearestNode(nonPC.position);
        this.destination = destination;
        nearestNodeDestination = FindNearestNode(destination);
        // FindClosestPath(nearestNodeNPC, nearestNodeDestination);
        /*
        find nearest node from npc (based on distance)
        find path with least number of nodes
            get adjacent nodes for each node by same x or same y
        get each Vector3Int between each node in the path
        Add them all sequentially 
        */
    }

    bool CheckStandable(Vector3Int targetPosition)
    {
        if (tileManager.tilesStandable.Contains(targetPosition)) return true;
        if (tileManager.tilesStandable.Contains(targetPosition + new Vector3Int(0, 0, 2))) return true;
        if (tileManager.tilesStandable.Contains(targetPosition + new Vector3Int(0, 0, -2))) return true;
        return false;
    }

    Vector3Int FindNearestNode(Vector3Int relativePosition)
    {
        Vector3Int nearestNode = Vector3Int.zero;
        int minDistance = int.MaxValue;
        foreach (Vector3Int node in tileManager.pathsNPCNodes)
        {
            int distance = Mathf.Abs(node.x - nonPC.position.x) + Mathf.Abs(node.x - nonPC.position.x);
            if (distance < minDistance)
            {
                nearestNode = node;
                minDistance = distance;
            } 
        }
        return nearestNode;
    }

    // List<Vector3Int> FindClosestPath(Vector3Int nodeStart, Vector3Int nodeDestination)
    // {
        
    // }

    // IEnumerator MoveOne(Vector3Int direction)
    // {
    //     float movementSpeed = 3f;
    //     float timeToMove = 1 / movementSpeed;
    //     if (timeToMove < 0) yield break;

    //     FaceDirection(direction);

    //     Vector3Int prevPosition = nonPC.position;
    //     Vector3Int tempPosition = prevPosition + direction;

    //     // Check if next position is standable
    //     if (tileManager.tilesStandable.Contains(tempPosition)) {}
    //     else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPosition.x, tempPosition.y, tempPosition.z - 2)))
    //     {
    //         tempPosition.z -= 2;
    //     }
    //     else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPosition.x, tempPosition.y, tempPosition.z + 2)))
    //     {
    //         tempPosition.z += 2;
    //     }
    //     else yield break;

    //     // Check if NPC is in the way
    //     if (GetComponent<PlayerNPCEncounter>().GetNPCAtPosition(tempPosition) != null) yield break;

    //     // All checks cleared --> handle movement
    //     isMoving = true;
    //     float elapsedTime = 0f;
    //     while(elapsedTime < timeToMove)
    //     {
    //         // Lerp moves from one position to the other in some amount of time.
    //         transform.position = Vector3.Lerp(floorMap.CellToWorld(prevPosition), floorMap.CellToWorld(tempPosition), (elapsedTime / timeToMove));
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
    //     transform.position = floorMap.CellToWorld(tempPosition);
    //     isMoving = false;
    // }

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
