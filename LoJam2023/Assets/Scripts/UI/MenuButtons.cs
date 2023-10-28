using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
    public void StartGame() {
        StartCoroutine(WaitToLoadFast());
    }

    public void StartGameDelayed() {
        StartCoroutine(WaitToLoad());
    }

    IEnumerator WaitToLoadFast() {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Level");
    }

    IEnumerator WaitToLoad() {
        yield return new WaitForSeconds(45f);
        SceneManager.LoadScene("Level");
    }

    public void QuitGame() {
        Application.Quit();
    }
}