using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
    public void StartGame() {
        SceneManager.LoadScene("Level");
    }

    public void StartGameDelayed() {
        StartCoroutine(WaitToLoad());
    }

    IEnumerator WaitToLoad() {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Level");
    }

    public void QuitGame() {
        Application.Quit();
    }
}