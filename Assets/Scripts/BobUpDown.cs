/*
 * Christian Rodriguez
 * May 5 2022
 * 
 * Makes the crystals bob up and down
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpDown : MonoBehaviour {

    private int amount = 10;
    private int direction = 1;

    private void Start() {
        StartCoroutine(Move());
    }

    private void FixedUpdate() {
        /*transform.Translate(new Vector2(0, 1f * Time.fixedDeltaTime * direction));
        --amount;
        if (amount <= 0) {
            amount = 20;
            direction *= -1;
        }*/
    }

    /*
     * Coroutine to move the crystal up and down
     */
    IEnumerator Move() {
        while (true) {
            if (GameManager.isPaused()) {
                // Don't move if paused
                yield return null;
            }
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + 1f * direction), Time.fixedDeltaTime);
            if (--amount <= 0) {
                // Move 10 units up, then 10 units down
                amount = 10;
                direction *= -1;
            }
            // Move every .2 seconds
            yield return new WaitForSeconds(0.2f);
        }
    }
}
