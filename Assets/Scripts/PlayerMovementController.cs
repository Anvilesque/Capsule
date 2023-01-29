using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private bool isMoving;
    private Vector3 origPos, targetPos, moveDirection;
    private float timeToMove;
    public float movementSpeed;
    public float angle;
    private float lastInput;
    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = Mathf.Sqrt(0.3125f); // To find hypotenuse: (0.5)^2 + (0.25)^2 = 0.3125
        timeToMove = 0.2f;
        angle = Mathf.Atan2(1f,2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputHoriz = Input.GetAxis("Horizontal");
        float inputVert = Input.GetAxis("Vertical");
        if(inputVert > 0 && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0)));
        if(inputHoriz < 0 && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(-Mathf.Cos(angle),Mathf.Sin(angle),0)));
        if(inputVert < 0  && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(-Mathf.Cos(angle),-Mathf.Sin(angle),0)));
        if(inputHoriz > 0 && !isMoving)
            StartCoroutine(MovePlayer(new Vector3(Mathf.Cos(angle),-Mathf.Sin(angle),0)));
    }
    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0f;
        origPos = transform.position;
        targetPos = origPos + direction*movementSpeed;
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
