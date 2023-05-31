using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private Tilemap floorMap, wallMap, transitionMap, interactableMap;
    private TileManager tileManager;
    private TaskManager taskManager;
    private PlayerTransition playerTransition;
    private SpriteRenderer playerSprite;
    [SerializeField] private List<Sprite> sprites;
    private static int LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3;

    public bool isMoving { get; private set; }
    public Vector3Int currentPos { get; private set; }
    public Vector3Int currentlyFacing { get; private set; }
    public float timeToMove;
    public float movementSpeed;
    public bool canMove;

    private List<string> lastDirection;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        taskManager = FindObjectOfType<TaskManager>();
        playerTransition = GetComponent<PlayerTransition>();
        floorMap = tileManager.floorMap;
        wallMap = tileManager.wallMap;
        transitionMap = tileManager.transitionMap;
        interactableMap = tileManager.interactableMap;
        playerSprite = GetComponent<SpriteRenderer>();

        // movementSpeed = 5; // Set this in Editor
        // angle = Mathf.Atan(1/2f);
        transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), PlayerPrefs.GetFloat("PlayerZ", 2));
        FaceDirection(new Vector3Int(PlayerPrefs.GetInt("DirX", 0), PlayerPrefs.GetInt("DirY", 1), 0));
        lastDirection = new List<string>();
        currentPos = floorMap.WorldToCell(transform.position);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        List<string> buttonNames = new List<string>() { "Left", "Right", "Up", "Down" };
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
            StartCoroutine(MovePlayer(Vector3Int.left));
        else if (Input.GetButton("Right") && lastDirection[lastDirection.Count - 1] == "Right")
            StartCoroutine(MovePlayer(Vector3Int.right));
        else if (Input.GetButton("Up") && lastDirection[lastDirection.Count - 1] == "Up")
            StartCoroutine(MovePlayer(Vector3Int.up));
        else if (Input.GetButton("Down") && lastDirection[lastDirection.Count - 1] == "Down")
            StartCoroutine(MovePlayer(Vector3Int.down));
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        if (!canMove) yield break;
        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        FaceDirection(direction);
        currentlyFacing = direction;

        Vector3Int prevPos = currentPos;
        Vector3Int tempPos = prevPos + direction;

        // Check if next position is standable
        if (tileManager.tilesStandable.Contains(tempPos)) { }
        else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPos.x, tempPos.y, tempPos.z - 2)))
        {
            tempPos.z -= 2;
        }
        else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPos.x, tempPos.y, tempPos.z + 2)))
        {
            tempPos.z += 2;
        }
        else yield break;

        // Check if NPC is in the way
        if (GetComponent<PlayerNPCEncounter>().GetNPCAtPosition(tempPos) != null) yield break;

        // All checks cleared --> handle movement
        isMoving = true;
        float elapsedTime = 0f;
        // Note: Currently updates player's currentPos BEFORE they actually get there; I think this is preferable
        // when it comes to NPC interactions, but it could be changed.
        currentPos = tempPos;
        while (elapsedTime < timeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(floorMap.CellToWorld(prevPos), floorMap.CellToWorld(tempPos), (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = floorMap.CellToWorld(currentPos);
        playerTransition.HandleTeleport();
        isMoving = false;
    }

    public void FaceDirection(Vector3Int direction)
    {
        int directionIndex =
            direction == Vector3Int.up ? UP :
            direction == Vector3Int.down ? DOWN :
            direction == Vector3Int.left ? LEFT :
            direction == Vector3Int.right ? RIGHT : -1;
        if (directionIndex == -1) return;
        playerSprite.sprite = sprites[directionIndex];
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void UpdateCurrentPosition()
    {
        currentPos = floorMap.WorldToCell(transform.position);
    }

    public void UpdateCurrentPosition(Vector3Int newPos)
    {
        currentPos = newPos;
    }
}
