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
        Vector2 dir = rend.flipX ? Vector2.left : Vector2.right;
        float dist = transform.lossyScale.x / 10;
        Debug.DrawRay(transform.position, dir * dist, Color.red, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, ground);
        if (hit.collider == null) {
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
        flip();
        onCollisionStayDisabled = false;
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (onCollisionStayDisabled || !collision.collider.CompareTag("Enemy"))
            return;
        collision.collider.GetComponent<EnemyController>().flip();
        onCollisionStayDisabled = true;
    }
    
    public void flip() {
        direction *= -1;
        rend.flipX = !rend.flipX;
    }

    public bool takeDamage(float damage) {
        rend.color = new Color(1f, 0.5f, 0.5f);
        StartCoroutine(resetColor());

        if ((health -= damage) <= 0) {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    private IEnumerator resetColor() {
        yield return new WaitForSeconds(0.15f);
        rend.color = new Color(1f, 1f, 1f);
    }
}
