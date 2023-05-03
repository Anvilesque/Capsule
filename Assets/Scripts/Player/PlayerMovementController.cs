using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMovementController : MonoBehaviour
{
    private Tilemap floorMap, wallMap, transitionMap, interactableMap;
    private TileManager tileManager;
    private TaskManager taskManager;
    private SpriteRenderer playerSprite;
    [SerializeField] private List<Sprite> sprites;
    private static int LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3;

    public bool isMoving {get; private set;}
    public Vector3 currentPos {get; private set;}
    public float timeToMove;
    public float movementSpeed;
    public bool allowMovement;

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
        currentPos = transform.position;
        allowMovement = true;
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
        if (!allowMovement) yield break;
        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        switch (direction)
        {
            case "Left": { playerSprite.sprite = sprites[LEFT]; break; }
            case "Right": { playerSprite.sprite = sprites[RIGHT]; break; }
            case "Up": { playerSprite.sprite = sprites[UP]; break; }
            case "Down": { playerSprite.sprite = sprites[DOWN]; break; }
        }

        Vector3 prevPos = transform.position;
        Vector3 tempPos = prevPos + distance;

        // Get Tilemap coords of next position
        Vector3Int tempPosGrid = floorMap.WorldToCell(tempPos);

        // Check if next position is standable
        if (!tileManager.tilesStandable.Contains(tempPosGrid)) yield break;

        // All checks cleared --> handle movement
        currentPos = tempPos;
        isMoving = true;
        float elapsedTime = 0f;
        while(elapsedTime < timeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(prevPos, currentPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = currentPos;
        HandleTeleport(true);
        isMoving = false;
    }

    public void HandleTeleport(bool fade)
    {
        TileData data = tileManager.GetTileData(transitionMap, transitionMap.WorldToCell(currentPos));
        
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
        Vector3 newCoords = floorMap.CellToWorld(data.newPos);
        transform.position += newCoords;
        yield return new WaitForSeconds(2f);
        EnableMovement();
        fadeController.FadeOut();
    }
    
    public void DisableMovement()
    {
        allowMovement = false;
    }
    public void EnableMovement()
    {
        allowMovement = true;
    }
}
