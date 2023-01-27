using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemenController : MonoBehaviour
{
    private bool IsMoving;
    private Vector3 OrigPos, TargetPos, MoveDirection;
    private float TimeToMove = 0.2f;
    public float MovementSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W) && !IsMoving)
            StartCoroutine(MovePlayer(new Vector3(1,1,0)));
        if(Input.GetKey(KeyCode.A) && !IsMoving)
            StartCoroutine(MovePlayer(new Vector3(-1,1,0)));
        if(Input.GetKey(KeyCode.S) && !IsMoving)
            StartCoroutine(MovePlayer(new Vector3(-1,-1,0)));
        if(Input.GetKey(KeyCode.D) && !IsMoving)
            StartCoroutine(MovePlayer(new Vector3(1,-1,0)));
    }
    private IEnumerator MovePlayer(Vector3 Direction)
    {
        IsMoving = true;
        float ElapsedTime = 0f;
        OrigPos = transform.position;
        TargetPos = OrigPos + Direction;
        while(ElapsedTime < TimeToMove)
        {
            // Lerp moves from one position to the other in some amount of time.
            transform.position = Vector3.Lerp(OrigPos, TargetPos, (ElapsedTime/TimeToMove));
            ElapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = TargetPos;
        IsMoving = false;
    }
}
