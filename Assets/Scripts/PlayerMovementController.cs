using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovementController : MonoBehaviour
{
    public Tilemap floorMap;
    public Tilemap wallMap;
    public Tilemap transitionMap;
    public Tilemap interactableMap;
    public TileManager tileManager;

    private bool isMoving;
    private Vector3 prevPosPoint, prevPosWorld, currentPosPoint, currentPosWorld, moveDirection;
    private float timeToMove;
    public float movementSpeed;
    // private float angle;

    private List<string> lastDirection;
    // Start is called before the first frame update
    void Start()
    {
        // movementSpeed = 5; // Set this in Editor
        // angle = Mathf.Atan(1/2f);
        lastDirection = new List<string>();
        tileManager = FindObjectOfType<TileManager>();
        prevPosPoint = transform.position;
        prevPosWorld = transform.position + new Vector3(0, -2 * tileManager.distY, 0); // // Player is rendered as being on (1, 1)
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        CheckInteractables();
    }
    
    private void UpdateMovement()
    {
        float inputHoriz = Input.GetAxis("Horizontal");
        float inputVert = Input.GetAxis("Vertical");
        if (Input.GetKeyDown("w"))
            lastDirection.Add("up");
        if(Input.GetKeyDown("a"))
            lastDirection.Add("left");
        if(Input.GetKeyDown("s"))
            lastDirection.Add("down");
        if(Input.GetKeyDown("d"))
            lastDirection.Add("right");
        if (Input.GetKeyUp("w"))
            lastDirection.Remove("up");
        if (Input.GetKeyUp("a"))
            lastDirection.Remove("left");
        if (Input.GetKeyUp("s"))
            lastDirection.Remove("down");
        if (Input.GetKeyUp("d"))
            lastDirection.Remove("right");

        if (lastDirection.Count == 0) return;
        if (isMoving) return;
        if(inputVert > 0 && lastDirection[lastDirection.Count-1] == "up")
            StartCoroutine(MovePlayer(new Vector3(tileManager.distX, tileManager.distY, 0)));
        else if(inputHoriz < 0 && lastDirection[lastDirection.Count-1] == "left")
            StartCoroutine(MovePlayer(new Vector3(-tileManager.distX, tileManager.distY, 0)));
        else if(inputVert < 0  && lastDirection[lastDirection.Count-1] == "down")
            StartCoroutine(MovePlayer(new Vector3(-tileManager.distX, -tileManager.distY, 0)));
        else if(inputHoriz > 0 && lastDirection[lastDirection.Count-1] == "right")
            StartCoroutine(MovePlayer(new Vector3(tileManager.distX, -tileManager.distY, 0)));
    }

    private void CheckInteractables()
    {
        if (isMoving) return;
        Vector3Int tileUp = tileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(0, -1, 0);
        Vector3Int tileDown = tileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(0, +1, 0);
        Vector3Int tileLeft = tileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(-1, 0, 0);
        Vector3Int tileRight = tileManager.WorldCoordsToGridCoords(currentPosWorld) + new Vector3Int(+1, 0, 0);
        List<Vector3Int> adjacentTiles = new List<Vector3Int>() {tileUp, tileDown, tileLeft, tileRight};
        foreach (Vector3Int tile in adjacentTiles)
        {
            Vector3Int tempTile = tile;
            for (int i = interactableMap.cellBounds.zMin; i <= interactableMap.cellBounds.zMax; i++)
            {
                tempTile.z = i;
                if (interactableMap.HasTile(tempTile))
                {
                    // Handle interactable here
                }
            }
        }
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {

        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        prevPosPoint = transform.position;
        prevPosWorld = transform.position + new Vector3(0, -2 * tileManager.distY, 0); // Player is rendered as being on (1, 1)
        currentPosPoint = prevPosPoint + direction;
        currentPosWorld = prevPosWorld + direction;

        // Get Tilemap coords of next position
        Vector3Int currentPosGrid = tileManager.WorldCoordsToGridCoords(currentPosWorld);  // https://clintbellanger.net/articles/isometric_math/
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
            TileData data = tileManager.GetTileData(currentPosGrid);
            if(data)
                transform.position = data.newPos;
        }
    }
}
