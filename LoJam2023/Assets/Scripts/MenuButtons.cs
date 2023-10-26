using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame() {
        SceneManager.LoadScene("PlayerTesting");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
