using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private bool isMoving;
    private Vector3 origPos, targetPos, moveDirection;
    private float timeToMove = 0.2f;
    public float movementSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W) && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(1,1,0)));
        if(Input.GetKey(KeyCode.A) && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(-1,1,0)));
        if(Input.GetKey(KeyCode.S) && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(-1,-1,0)));
        if(Input.GetKey(KeyCode.D) && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(1,-1,0)));
    }
    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0f;
        origPos = transform.position;
        targetPos = origPos + direction;
        while(elapsedTime < timeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
