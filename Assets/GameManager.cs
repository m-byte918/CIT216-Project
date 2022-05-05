/*
 * Christian Rodriguez
 * May 5 2022
 * 
 * Manages global game information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private static int restarts = 0;
    private static bool paused = true;

    public static GameObject instructions;
    public static Button youDiedRestartButton;
    public static Button youWonRestartButton;

    public void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        instructions = GameObject.Find("Instructions");

        // I can't figure out a more efficient way to get a button that is disabled in the inspector
        foreach (GameObject g in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            // Get restart button
            if (g.name == "DiedRestartButton") {
                youDiedRestartButton = g.GetComponent<Button>();
                break;
            }
        }
        foreach (GameObject g in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            // Get won restart button
            if (g.name == "WonRestartButton") {
                youWonRestartButton = g.GetComponent<Button>();
                break;
            }
        }
        // Add listeners to buttons
        Button instructionsStartBtn = instructions.transform.Find("Button").GetComponent<Button>();
        instructionsStartBtn.onClick.AddListener(togglePaused);
        instructionsStartBtn.onClick.AddListener(delegate { instructions.SetActive(false); });
        youDiedRestartButton.onClick.AddListener(restart);
        youWonRestartButton.onClick.AddListener(restart);
    }

    /*
     * Toggle the pause state of the game
     */
    public static void togglePaused() {
        paused = !paused;
    }

    /*
     * Returns whether or not the game is paused
     */
    public static bool isPaused() {
        return paused;
    }

    /*
     * Restarts the game
     */
    public static void restart() {
        ++restarts;
        Debug.Log("Restarts: " + restarts);
        SceneManager.LoadScene(0);
    }
}
