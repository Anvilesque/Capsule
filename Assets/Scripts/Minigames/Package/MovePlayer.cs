using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour
{
    private Vector2 playerVelocity;
    private float playerSpeed = 10.0f;
    public int boxesCaught = 0;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite frontSprite;
    public Sprite catchSprite;
    private float caughtTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Left"))
        {
            playerVelocity.x = -playerSpeed;
            sprite.sprite = leftSprite;
        }
        else if (Input.GetButton("Right"))
        {
            playerVelocity.x = playerSpeed;
            sprite.sprite = rightSprite;
        }
        else
        {
            playerVelocity.x = 0;
            sprite.sprite = frontSprite;
        }
        if (caughtTime > 0)
            sprite.sprite = catchSprite;
        rb.velocity = playerVelocity;
        caughtTime -= Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Package")
        {
            caughtTime = 0.3f;
            Destroy(collision.gameObject);
            int balance = PlayerPrefs.GetInt("money", 0);
            PlayerPrefs.SetInt("money", balance + 5);
            StartCoroutine("Indicator");
        }
    }
    IEnumerator Indicator()
    {
        float timer = 0f;
        float duration = 1f;
        float randX = Random.Range(-3.0f, 3.0f);
        float randY = Random.Range(-2.0f, 2.0f);
        GameObject indicator = Instantiate((GameObject)Resources.Load("Prefabs/Minigames/Packaging Minigame/Indicator"), new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z), Quaternion.identity, FindObjectOfType<Canvas>().transform);
        while (indicator.GetComponent<Image>().color.a > 0)
        {
            indicator.GetComponent<Image>().color = new Color(1, 1, 1, 1 - (timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(indicator);
    }
}
