using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    private SpriteRenderer rend;
    private Animator anim;
    public Transform groundCheck;
    public GameObject fire;
    public Transform firePosition;
    public GameObject youDiedImg;
    public GameObject youWonImg;
    public Text wallet;
    public RawImage healthBar;
    public Texture[] healthBarTextures;
    public AudioSource gunshotSound;
    
    private float h;
    private bool jump = false;
    private int jumps= 0;
    private bool isGrounded = true; 
    public float moveForce = 150f;
    public float maxSpeed = 5f;
    public float jumpForce = 400f;
    private float fireRate = 0.3f;
    private float nextFire = 0f;
    private bool facingRight = true;
    public int health = 6;
    private int coinCount = 0;
    public int enemiesKilled = 0;
    private float lastDamageTime = 0f;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (GameManager.isPaused())
            return;
        h = Input.GetAxis("Horizontal");

        // make player stop quicker
        if (body.velocity.x != 0 && Mathf.Abs(h) < 0.5)
            h = 0;

        // make sprite face correct direction
        if ((h > 0 && !facingRight) || (h < 0 && facingRight))
            flip();

        Debug.DrawRay(groundCheck.position, new Vector2(0, 0.2f), Color.red, 1f);
        RaycastHit2D grounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f); //0.2f is distance

        if (grounded.collider != null) {
            // Player has landed
            isGrounded = true;
            anim.SetBool("IsJumping", false);  
        } else {
            isGrounded = false;
            anim.SetBool("IsJumping", true);
        }

        // set player anim
        if (h != 0 && isGrounded)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);

        if (Input.GetButtonDown("Jump")) {
            if (jumps >= 2) {
                if (isGrounded)
                    jumps = 0;
            } else {
                jump = true;
            }
        }
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) {
            Instantiate(fire, firePosition);
            nextFire = Time.time + fireRate;
            gunshotSound.Play();
        }
    }

    void FixedUpdate()
    {
        if (GameManager.isPaused())
            return;
        body.AddForce(Vector2.right * h * moveForce);

        // but limit how fast the player can move
        if (Mathf.Abs(body.velocity.x) > maxSpeed)
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);

        if (jump) {
            body.AddForce(Vector2.up * jumpForce);
            jump = false;
            ++jumps;
        }
        // make player fall faster
        if (body.velocity.y >= 0)
            body.gravityScale = 1f;
        else
            body.gravityScale = 5f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Pickup")) {
            Destroy(collision.gameObject);

            // Assume it is a coin
            ++coinCount;
            wallet.text = "Crystals: " + coinCount;
            checkEndGame();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            takeDamage(1);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            takeDamage(1);
        }
    }

    private void pauseGameAndPlayer() {
        body.gravityScale *= 0;
        body.velocity = Vector2.zero;
        GameManager.togglePaused();
    }

    public void checkEndGame() {
        if (coinCount < 15 || enemiesKilled < 10)
            return;
        youWonImg.SetActive(true);
        pauseGameAndPlayer();
    }

    public void takeDamage(int damage) {
        if (Time.time - lastDamageTime < 1f)
            return;
        health -= damage;
        if (health <= 0) {
            youDiedImg.SetActive(true);
            pauseGameAndPlayer();
            healthBar.texture = healthBarTextures[0];
        } else {
            healthBar.texture = healthBarTextures[health - 1];
        }
        lastDamageTime = Time.time;
    }

    private void flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public Vector3 getDirection() {
        return facingRight ? Vector3.right : Vector3.left;
    }
}
