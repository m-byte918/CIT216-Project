/*
 * Christian Rodriguez
 * May 5 2022
 * 
 * Controls the behavior of boundaries (lava)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryController : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        // Kill any entity that touches boundary
        if (collision.CompareTag("Enemy")) {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            enemy.takeDamage(enemy.health);
        }
        else if (collision.CompareTag("Player")) {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.takeDamage(player.health);
        }
    }
}
