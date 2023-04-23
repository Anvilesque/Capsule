using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private Vector2 playerVelocity;
    private float playerSpeed = 10.0f;
    public int boxesCaught = 0;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Left"))
            playerVelocity.x = -playerSpeed;
        else if(Input.GetButton("Right"))
            playerVelocity.x = playerSpeed;
        else
            playerVelocity.x = 0;
        rb.velocity = playerVelocity;
        
    }
    private void OnTriggerEnter2D(Collider2D collission)
    {
        if(collission.gameObject.tag == "Package")
        {
            Destroy(collission.gameObject);
            boxesCaught += 1;
            Debug.Log(boxesCaught);
        }
    }
}
