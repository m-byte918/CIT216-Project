/*
 * Christian Rodriguez
 * May 5 2022
 * 
 * Controls 'bullet' behavior
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    public float speed = 12f;
    private Vector3 direction = Vector3.right;
    private Rigidbody2D body;
    private PlayerController player;

    // Start is called before the first frame update
    void Start() {
        // Cache
        body = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p.GetComponent<PlayerController>();
        direction = player.getDirection();
        transform.SetParent(null);
        Invoke("shinda", 2f);
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    /*
     * Destroys the bullet
     */
    void shinda() {
        if (gameObject != null)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
            return;
        shinda();

        if (collision.CompareTag("Enemy") && collision.GetComponent<EnemyController>().takeDamage(10)) {
            // Enemy take damage
            ++player.enemiesKilled;
            player.checkEndGame();
        }
    }
}
