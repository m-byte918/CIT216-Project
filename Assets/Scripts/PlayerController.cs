using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    private SpriteRenderer rend;
    private Animator anim;
    private Tile rockTile;
    private Texture2D grottoTilesTexture;

    public Transform groundCheck;
    public GameObject fire;
    public Transform firePosition;
    public GameObject youDiedImg;
    public GameObject youWonImg;
    public Text wallet;
    public RawImage healthBar;
    public Texture[] healthBarTextures;
    public AudioSource gunshotSound;
    public Tilemap pillarTilemap;
    
    private float h;
    private bool jump = false;
    private bool isGrounded = true;
    private bool isLaunchable = false;
    private float fireRate = 0.3f;
    private float nextFire = 0f;
    private bool facingRight = true;
    private int coinCount = 0;
    private float lastDamageTime = 0f;

    public float moveForce = 150f;
    public float maxSpeed = 5f;
    public float jumpForce = 400f;
    public int health = 6;
    public int enemiesKilled = 0;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Load & cache rock texture & tile
        rockTile = ScriptableObject.CreateInstance<Tile>();
        grottoTilesTexture = Resources.Load<Texture2D>("grotto-tiles");
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

            if (grounded.collider.CompareTag("Spikable")) {
                isLaunchable = true;
            }
        } else {
            isGrounded = false;
            isLaunchable = false;
            anim.SetBool("IsJumping", true);
        }

        // set player anim
        if (h != 0 && isGrounded)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);

        if (Input.GetButtonDown("Jump") && isGrounded) {
            jump = true;
            Launch();
        }
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) {
            Instantiate(fire, firePosition);
            nextFire = Time.time + fireRate;
            gunshotSound.Play();
        }
    }

    void Launch() {
        StartCoroutine(erectPillar(
            Mathf.RoundToInt(transform.position.x) - 1,
            Mathf.RoundToInt(transform.position.x), 
            Mathf.RoundToInt(transform.position.y) - 1
        ));
        Debug.Log("Launched");
    }

    IEnumerator erectPillar(int lx, int rx, int y) {
        int blocks = 0;
        while (blocks < 3) {
            // Pillar will be 3 blocks high
            int blockHeight = 1;
            while (blockHeight < 17) {
                // Build a pillar 1 pixel at a time (probably a more efficient way to do this, but i dont have time lol)
                rockTile.sprite = Sprite.Create(grottoTilesTexture,
                    new Rect(80, 32, 16, blockHeight),
                    new Vector2(0.5f, 1/blockHeight),
                16, 1);
                pillarTilemap.SetTile(new Vector3Int(lx, y + blocks, 1), rockTile); // left side
                pillarTilemap.SetTile(new Vector3Int(rx, y + blocks, 1), rockTile); // right side
                pillarTilemap.RefreshTile(new Vector3Int(lx, y + blocks, 1));
                pillarTilemap.RefreshTile(new Vector3Int(rx, y + blocks, 1));
                ++blockHeight;
                yield return new WaitForSeconds(0.0001f);
            }
            ++blocks;
        }
        // Rock pillars will give twice the force of a regular jump
        body.AddForce(Vector2.up * jumpForce * 2);
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
