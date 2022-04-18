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

    IEnumerator Move() {
        while (true) {
            if (GameManager.isPaused())
                yield return null;
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + 1f * direction), Time.fixedDeltaTime);
            if (--amount <= 0) {
                amount = 10;
                direction *= -1;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
