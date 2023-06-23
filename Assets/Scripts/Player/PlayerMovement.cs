using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private SaveManager saveManager;
    private SaveData saveData;
    private TileManager tileManager;
    private Tilemap floorMap;
    private Tilemap stairsMap;
    private TaskManager taskManager;
    private PlayerTransition playerTransition;
    private SpriteRenderer playerSprite;
    private PlayerNPCEncounter playerNPCEncounter;
    [SerializeField] private List<Sprite> sprites;
    private static int LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3;
    private static float STAIRS_OFFSET = -0.15f;

    public bool isMoving { get; private set; }
    private bool isRunningCoroutine;
    public Vector3Int currentPos;
    public Vector3Int currentlyFacing;
    public int numFixedUpdatesToMoveOne;
    private bool canMove;
    public Vector3Int defaultHomePosition;
    public Vector3Int defaultBedPosition;

    private List<string> lastDirection = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveData = saveManager.myData;
        tileManager = FindObjectOfType<TileManager>();
        taskManager = FindObjectOfType<TaskManager>();
        playerTransition = GetComponent<PlayerTransition>();
        floorMap = tileManager.floorMap;
        stairsMap = tileManager.stairsMap;
        playerSprite = GetComponent<SpriteRenderer>();
        playerNPCEncounter = GetComponent<PlayerNPCEncounter>();

        // movementSpeed = 5; // Set this in Editor
        // angle = Mathf.Atan(1/2f);
        transform.position = saveData.playerTransformPosition;
        FaceDirection(saveData.playerCurrentlyFacing);
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
        if (CapsuleResponseViewer.isWriting) return;
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
        if (isRunningCoroutine) return;
        if (Input.GetButton("Left") && lastDirection[lastDirection.Count - 1] == "Left")
            StartCoroutine(MovePlayer(Vector3Int.left));
        else if (Input.GetButton("Right") && lastDirection[lastDirection.Count - 1] == "Right")
            StartCoroutine(MovePlayer(Vector3Int.right));
        else if (Input.GetButton("Up") && lastDirection[lastDirection.Count - 1] == "Up")
            StartCoroutine(MovePlayer(Vector3Int.up));
        else if (Input.GetButton("Down") && lastDirection[lastDirection.Count - 1] == "Down")
            StartCoroutine(MovePlayer(Vector3Int.down));
    }

    private string DirectionToStr(Vector3Int direction)
    {
        return direction == Vector3Int.left ? "Left" :
               direction == Vector3Int.right ? "Right" :
               direction == Vector3Int.up ? "Up" :
               direction == Vector3Int.down ? "Down" : null;
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        isRunningCoroutine = true;
        if (!canMove) { isRunningCoroutine = false; yield break; }
        if (numFixedUpdatesToMoveOne < 0) { isRunningCoroutine = false; yield break; }

        FaceDirection(direction);

        Vector3Int prevPos = currentPos;
        Vector3Int tempPos = prevPos + direction;

        // Check if next position is standable
        if (tileManager.tilesStandable.Contains(tempPos)) {}
        else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPos.x, tempPos.y, tempPos.z - 2)))
        {
            tempPos.z -= 2;
        }
        else if (tileManager.tilesStandable.Contains(new Vector3Int(tempPos.x, tempPos.y, tempPos.z + 2)))
        {
            tempPos.z += 2;
        }
        else { isRunningCoroutine = false; yield break; }

        // Check if NPC is in the way
        float npcBypassTimer = 0f;
        float npcBypassThreshold = 1f;
        while (playerNPCEncounter.GetNPCAtPosition(tempPos) != null && npcBypassTimer < npcBypassThreshold)
        {
            if (Input.GetButton(DirectionToStr(direction)))
            {
                npcBypassTimer += Time.deltaTime;
                yield return null;
            }
            else { isRunningCoroutine = false; yield break; }
        }

        // All checks cleared --> handle movement
        isMoving = true;
        bool prevOnStairs = tileManager.ScanForTile(stairsMap, prevPos) ? true : false;
        bool currentOnStairs = tileManager.ScanForTile(stairsMap, tempPos) ? true : false;
        // Note: Currently updates player's currentPos BEFORE they actually get there; I think this is preferable
        // when it comes to NPC interactions, but it could be changed.
        currentPos = tempPos;
        Vector3 prevPosWorld  = floorMap.CellToWorld(prevPos);
        Vector3 currentPosWorld = floorMap.CellToWorld(currentPos);
        prevPosWorld.y += prevOnStairs ? STAIRS_OFFSET : 0f;
        currentPosWorld.y += currentOnStairs ? STAIRS_OFFSET : 0f;
        float movementCompletePercentage = 0f;
        while (movementCompletePercentage < 1)
        {
            // Lerp moves from one position to the other in some amount of time.
            movementCompletePercentage += 1.0f / numFixedUpdatesToMoveOne;
            transform.position = Vector3.Lerp(prevPosWorld, currentPosWorld, Mathf.Clamp01(movementCompletePercentage));
            yield return new WaitForFixedUpdate();
        }
        playerTransition.HandleTeleportFromTransitionTilemap();
        isMoving = false;
        isRunningCoroutine = false;
    }

    public void FaceDirection(Vector3Int direction)
    {
        currentlyFacing = direction;
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
}
