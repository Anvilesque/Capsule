using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovementController : MonoBehaviour
{
    private Tilemap floorMap;
    private Tilemap wallMap;
    private Tilemap transitionMap;
    private Tilemap interactableMap;
    private TileManager tileManager;

    private bool isMoving;
    private Vector3 prevPosPoint, prevPosWorld, currentPosPoint, currentPosWorld, moveDirection;
    public Vector3Int currentPosGrid;
    public float timeToMove;
    public float movementSpeed;
    public bool canMove;

    private List<string> lastDirection;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        wallMap = tileManager.wallMap;
        transitionMap = tileManager.transitionMap;
        interactableMap = tileManager.interactableMap;

        // movementSpeed = 5; // Set this in Editor
        // angle = Mathf.Atan(1/2f);
        lastDirection = new List<string>();
        prevPosPoint = transform.position;
        prevPosWorld = transform.position + new Vector3(0, -2 * TileManager.distY, 0); // // Player is rendered as being on (1, 1)
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        CheckInteractables();
    }
    
    private void UpdateMovement()
    {
        List<string> buttonNames = new List<string>() {"Left", "Right", "Up", "Down"};
        foreach (string button in buttonNames)
        {
            if (Input.GetButtonDown(button))
            {
                lastDirection.Add(button);
                // Debug.Log("Button check: pressed " + button);
            }
            if (Input.GetButtonUp(button))
            {
                lastDirection.Remove(button);
                // Debug.Log("Button check: released " + button);
            }
        }

        if (lastDirection.Count == 0) return;
        if (isMoving) return;
        if(Input.GetButton("Left") && lastDirection[lastDirection.Count-1] == "Left")
            StartCoroutine(MovePlayer(new Vector3(-TileManager.distX, TileManager.distY, 0)));
        else if(Input.GetButton("Right") && lastDirection[lastDirection.Count-1] == "Right")
            StartCoroutine(MovePlayer(new Vector3(TileManager.distX, -TileManager.distY, 0)));
        else if(Input.GetButton("Up") && lastDirection[lastDirection.Count-1] == "Up")
            StartCoroutine(MovePlayer(new Vector3(TileManager.distX, TileManager.distY, 0)));
        else if(Input.GetButton("Down") && lastDirection[lastDirection.Count-1] == "Down")
            StartCoroutine(MovePlayer(new Vector3(-TileManager.distX, -TileManager.distY, 0)));
    }

    private void UpdateTilemaps()
    {

    }

    private void CheckInteractables()
    {
        if (isMoving) return;
        Vector3Int tileUp = TileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(0, -1, 0);
        Vector3Int tileDown = TileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(0, +1, 0);
        Vector3Int tileLeft = TileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(-1, 0, 0);
        Vector3Int tileRight = TileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(+1, 0, 0);
        List<Vector3Int> adjacentTiles = new List<Vector3Int>() {tileUp, tileDown, tileLeft, tileRight};
        foreach (Vector3Int tile in adjacentTiles)
        {
            Vector3Int tempTile = tile;
            for (int i = interactableMap.cellBounds.zMin; i <= interactableMap.cellBounds.zMax; i++)
            {
                tempTile.z = i;
                if (interactableMap.HasTile(tempTile))
                {
                    string taskName = tileManager.GetTileData(interactableMap, tempTile).taskName;
                    // TaskManager.startTask(taskName);
                }
            }
        }
    }

    private IEnumerator MovePlayer(Vector3 distance)
    {
        if (!canMove) yield break;
        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        prevPosPoint = transform.position;
        prevPosWorld = transform.position + new Vector3(0, -2 * TileManager.distY, 0); // Player is rendered as being on (1, 1)
        currentPosPoint = prevPosPoint + distance;
        currentPosWorld = prevPosWorld + distance;

        // Get Tilemap coords of next position
        currentPosGrid = TileManager.WorldCoordsToGridCoords(currentPosWorld);  // https://clintbellanger.net/articles/isometric_math/
        // Check for wall or interactable in front
        for (int i = wallMap.cellBounds.zMin; i <= wallMap.cellBounds.zMax; i++)
        {
            currentPosGrid.z = i;
            if (wallMap.HasTile(currentPosGrid)) yield break;
            if (interactableMap.HasTile(currentPosGrid)) yield break;
        }
        // Check for floor in front (floor is always z = 1)
        currentPosGrid.z = 1;
        if (!floorMap.HasTile(currentPosGrid) && !transitionMap.HasTile(currentPosGrid)) yield break;

        isMoving = true;
        float elapsedTime = 0f;
        while(elapsedTime < timeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(prevPosPoint, currentPosPoint, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = currentPosPoint;
        isMoving = false;
        if (elapsedTime > timeToMove)
        {
            TileData data = tileManager.GetTileData(transitionMap, currentPosGrid);
            if(data)
            {
                Vector3 newCoords =TileManager.GridCoordsToWorldCoords(data.newPos);
                transform.position += newCoords;
            }
        }
    }
    
    public void DisableMovement()
    {
        canMove = false;
    }
    public void EnableMovement()
    {
        canMove = true;
    }
}
