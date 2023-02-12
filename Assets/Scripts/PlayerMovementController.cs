using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovementController : MonoBehaviour
{
    public Tilemap floorMap;
    public Tilemap wallMap;
    public Tilemap transitionMap;

    private bool isMoving;
    private Vector3 origPos, targetPos, moveDirection;
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
    }

    // Update is called once per frame
    void Update()
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
            StartCoroutine(MovePlayer(new Vector3(TileManager.distX, TileManager.distY, 0)));
        else if(inputHoriz < 0 && lastDirection[lastDirection.Count-1] == "left")
            StartCoroutine(MovePlayer(new Vector3(-TileManager.distX, TileManager.distY, 0)));
        else if(inputVert < 0  && lastDirection[lastDirection.Count-1] == "down")
            StartCoroutine(MovePlayer(new Vector3(-TileManager.distX, -TileManager.distY, 0)));
        else if(inputHoriz > 0 && lastDirection[lastDirection.Count-1] == "right")
            StartCoroutine(MovePlayer(new Vector3(TileManager.distX, -TileManager.distY, 0)));
    }
    
    private IEnumerator MovePlayer(Vector3 direction)
    {

        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        origPos = transform.position;
        targetPos = origPos + direction;

        // Get Tilemap coords of next position
        Vector3Int targetPosGrid = TileManager.WorldCoordsToGridCoords(targetPos);  // https://clintbellanger.net/articles/isometric_math/
        targetPosGrid.x -= 1;                                                       // Subtract 1 b/c Player is rendered as being on (1, 1)
        targetPosGrid.y -= 1;
        // Check for wall in front
        for (int i = wallMap.cellBounds.zMin; i <= wallMap.cellBounds.zMax; i++)
        {
            targetPosGrid.z = i;
            if (wallMap.HasTile(targetPosGrid)) yield break;
        }
        // Check for floor in front (floor is always z = 1)
        targetPosGrid.z = 1;
        if (!floorMap.HasTile(targetPosGrid) && !transitionMap.HasTile(targetPosGrid)) yield break;

        isMoving = true;
        float elapsedTime = 0f;
        while(elapsedTime < timeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        if (elapsedTime > timeToMove)
        {
            /* TileBase tile = transitionMap.GetTile(targetPosGrid);
            if(!tile)
                yield return null;
            // transform.position = TileManager.dataFromTiles[tile].newPos; */
        }
    }
}
