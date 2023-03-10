using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMovementController : MonoBehaviour
{
    private Tilemap floorMap;
    private Tilemap wallMap;
    private Tilemap transitionMap;
    private Tilemap interactableMap;
    private TileManager tileManager;
    private TaskManager taskManager;
    private SpriteRenderer playerSprite;
    [SerializeField] private List<Sprite> sprites;
    private static int LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3;

    public bool isMoving {get; private set;}
    private Vector3 prevPosPoint, prevPosWorld, currentPosPoint, moveDirection;
    public Vector3 currentPosWorld {get; private set;}
    public Vector3Int currentPosGrid;
    public float timeToMove;
    public float movementSpeed;
    public bool canMove {get; private set;}

    private List<string> lastDirection;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        taskManager = FindObjectOfType<TaskManager>();
        floorMap = tileManager.floorMap;
        wallMap = tileManager.wallMap;
        transitionMap = tileManager.transitionMap;
        interactableMap = tileManager.interactableMap;
        playerSprite = GetComponent<SpriteRenderer>();

        // movementSpeed = 5; // Set this in Editor
        // angle = Mathf.Atan(1/2f);
        lastDirection = new List<string>();
        prevPosPoint = transform.position;
        prevPosWorld = transform.position + new Vector3(0, -2 * TileManager.distY, 0); // // Player is rendered as being on (1, 1)
        currentPosWorld = prevPosWorld;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
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
        if (Input.GetButton("Left") && lastDirection[lastDirection.Count - 1] == "Left")
            StartCoroutine(MovePlayer(new Vector3(-TileManager.distX, TileManager.distY, 0), "Left"));
        else if (Input.GetButton("Right") && lastDirection[lastDirection.Count - 1] == "Right")
            StartCoroutine(MovePlayer(new Vector3(TileManager.distX, -TileManager.distY, 0), "Right"));
        else if (Input.GetButton("Up") && lastDirection[lastDirection.Count - 1] == "Up")
            StartCoroutine(MovePlayer(new Vector3(TileManager.distX, TileManager.distY, 0), "Up"));
        else if (Input.GetButton("Down") && lastDirection[lastDirection.Count - 1] == "Down")
            StartCoroutine(MovePlayer(new Vector3(-TileManager.distX, -TileManager.distY, 0), "Down"));
    }

    private void UpdateTilemaps()
    {

    }

    private IEnumerator MovePlayer(Vector3 distance, string direction)
    {
        if (!canMove) yield break;
        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        if (direction == "Left")
        {
            playerSprite.sprite = sprites[LEFT];
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (direction == "Right")
        {
            playerSprite.sprite = sprites[RIGHT];
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (direction == "Up")
        {
            playerSprite.sprite = sprites[UP];
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else if (direction == "Down")
        {
            playerSprite.sprite = sprites[DOWN];
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        prevPosPoint = transform.position;
        prevPosWorld = transform.position + new Vector3(0, -2 * TileManager.distY, 0); // Player is rendered as being on (1, 1)
        Vector3 tempPosPoint = prevPosPoint + distance;
        Vector3 tempPosWorld = prevPosWorld + distance;

        // Get Tilemap coords of next position
        Vector3Int tempPosGrid = TileManager.WorldCoordsToGridCoords(tempPosWorld);  // https://clintbellanger.net/articles/isometric_math/
        // Check for wall or interactable in front
        for (int i = wallMap.cellBounds.zMin; i <= wallMap.cellBounds.zMax; i++)
        {
            tempPosGrid.z = i;
            if (wallMap.HasTile(tempPosGrid)) yield break;
            if (interactableMap.HasTile(tempPosGrid)) yield break;
        }
        // Check for floor in front (floor is always z = 1)
        tempPosGrid.z = 1;
        if (!floorMap.HasTile(tempPosGrid) && !transitionMap.HasTile(tempPosGrid)) yield break;

        // All checks cleared --> handle movement
        currentPosPoint = tempPosPoint;
        currentPosWorld = tempPosWorld;
        currentPosGrid = tempPosGrid;
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
        HandleTeleport(true);
        isMoving = false;
    }

    public void HandleTeleport(bool fade)
    {
        TileData data = tileManager.GetTileData(transitionMap, currentPosGrid);
        
        if (data)
        {
            StartCoroutine (TeleportFadeInOut(data));
        }
    }

    IEnumerator TeleportFadeInOut(TileData data)
    {
        DisableMovement();
        FadeController fadeController = FindObjectOfType<FadeController>();
        fadeController.FadeIn();
        while (fadeController.isFading) yield return null;
        Vector3 newCoords = TileManager.GridCoordsToWorldCoords(data.newPos);
        transform.position += newCoords;
        yield return new WaitForSeconds(2f);
        EnableMovement();
        fadeController.FadeOut();
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
