using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseScreen;

    private bool paused = false;

    // Start is called before the first frame update
    void Start() {
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            if (paused) {
                Pause();
            } else {
                Resume();
            }
        }
    }

    void Pause() {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    void Resume() {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
}
