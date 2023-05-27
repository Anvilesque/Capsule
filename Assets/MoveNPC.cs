using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveNPC : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap floorMap;
    private NonPC nonPC;
    private int stepsFromMovingForward;
    private Vector3Int destination;
    private Vector3Int testPosition;
    private List<Vector3Int> directionsTried;
    private List<Vector3Int> path;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        nonPC = GetComponent<NonPC>();
        directionsTried = new List<Vector3Int>();
    }

    void Update()
    {

    }

    public void MoveNPCToDest(Vector3Int destination)
    {
        directionsTried.Clear();
        this.destination = destination;
        testPosition = nonPC.position;
    }

    bool CheckStandable(Vector3Int targetPosition)
    {
        if (tileManager.tilesStandable.Contains(targetPosition)) return true;
        if (tileManager.tilesStandable.Contains(targetPosition + new Vector3Int(0, 0, 2))) return true;
        if (tileManager.tilesStandable.Contains(targetPosition + new Vector3Int(0, 0, -2))) return true;
        return false;
    }

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
