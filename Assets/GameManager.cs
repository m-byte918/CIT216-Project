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
     
        foreach (GameObject g in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            if (g.name == "DiedRestartButton") {
                youDiedRestartButton = g.GetComponent<Button>();
                break;
            }
        }
        foreach (GameObject g in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            if (g.name == "WonRestartButton") {
                youWonRestartButton = g.GetComponent<Button>();
                break;
            }
        }
        Button instructionsStartBtn = instructions.transform.Find("Button").GetComponent<Button>();
        instructionsStartBtn.onClick.AddListener(togglePaused);
        instructionsStartBtn.onClick.AddListener(delegate { instructions.SetActive(false); });
        youDiedRestartButton.onClick.AddListener(restart);
        youWonRestartButton.onClick.AddListener(restart);
    }

    public static void togglePaused() {
        paused = !paused;
    }

    public static bool isPaused() {
        return paused;
    }

    public static void restart() {
        ++restarts;
        Debug.Log("Restarts: " + restarts);
        SceneManager.LoadScene(0);
    }
}
