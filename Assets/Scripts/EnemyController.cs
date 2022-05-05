/*
 * Christian Rodriguez
 * May 5 2022
 * 
 * Controls the behavior of enemies
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public LayerMask ground;
    public int direction = -1;
    public SpriteRenderer rend;
    public float health = 100f;
    private bool onCollisionStayDisabled = false;

    private void Start() {
        rend = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (GameManager.isPaused())
            return;
        Vector2 dir = rend.flipX ? Vector2.left : Vector2.right; // Raycast direction based off the direction the enemy is facing
        float dist = transform.lossyScale.x / 10; // Distance based on the enemy's width
        //Debug.DrawRay(transform.position, dir * dist, Color.red, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, ground);
        if (hit.collider == null) {
            // No collision with any ground, try finding for a flipboundary instead
            hit = Physics2D.Raycast(transform.position, dir, dist, 1);
        }
        if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.CompareTag("FlipBoundary")) {
            // Hit flip boundary
            flip();
            onCollisionStayDisabled = false;
        }
        transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x + 1 * direction, transform.position.y), Time.fixedDeltaTime);
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.collider.CompareTag("Enemy"))
            return;
        // Flip this enemy
        flip();
        onCollisionStayDisabled = false;
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (onCollisionStayDisabled || !collision.collider.CompareTag("Enemy"))
            return;
        // Flip the other enemy
        collision.collider.GetComponent<EnemyController>().flip();
        onCollisionStayDisabled = true;
    }
    
    /*
     * Flip the sprite renderer onlong the X axis
     */
    public void flip() {
        direction *= -1;
        rend.flipX = !rend.flipX;
    }

    /*
     * Subtracts the provided amount of damage from the enemy's health
     */
    public bool takeDamage(float damage) {
        rend.color = new Color(1f, 0.5f, 0.5f); // Flash red
        StartCoroutine(resetColor());

        if ((health -= damage) <= 0) {
            // Dead
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    /*
     * Coroutine to restore the enemy's original color after .15 seconds
     */
    private IEnumerator resetColor() {
        yield return new WaitForSeconds(0.15f);
        rend.color = new Color(1f, 1f, 1f);
    }
}
