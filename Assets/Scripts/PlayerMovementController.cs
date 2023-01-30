using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
   private bool isMoving;
    private Vector3 origPos, targetPos, moveDirection;
    private float timeToMove;
    public float movementSpeed;
    // private float angle;
    private float distX;
    private float distY;
    
    private List<string> lastDirection;
    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 5;
        // angle = Mathf.Atan(1/2f);
        distX = 0.5f; // Cell width = 1 --> X-dist to next cell = half of 1
        distY = 0.25f; // Cell height = 0.5 --> Y-dist to next cell = half of 0.5
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
            StartCoroutine(MovePlayer(new Vector3(distX, distY, 0)));
        else if(inputHoriz < 0 && lastDirection[lastDirection.Count-1] == "left")
            StartCoroutine(MovePlayer(new Vector3(-distX, distY, 0)));
        else if(inputVert < 0  && lastDirection[lastDirection.Count-1] == "down")
            StartCoroutine(MovePlayer(new Vector3(-distX, -distY, 0)));
        else if(inputHoriz > 0 && lastDirection[lastDirection.Count-1] == "right")
            StartCoroutine(MovePlayer(new Vector3(distX, -distY, 0)));
    }
    private IEnumerator MovePlayer(Vector3 direction)
    {
        timeToMove = 1 / movementSpeed;
        if (timeToMove < 0) yield break;

        origPos = transform.position;
        targetPos = origPos + direction;
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
    }
}
