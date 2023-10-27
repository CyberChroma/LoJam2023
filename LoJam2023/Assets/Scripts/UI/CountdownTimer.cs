using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    private float totalTimeInSeconds = 300f; // 5 minutes in seconds

    void Start() {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown() {
        float currentTime = totalTimeInSeconds;

        while (currentTime >= 0) {
            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // Update the TextMeshPro text
            timerText.text = $"{minutes:D2}:{seconds:D2}";

            // Wait for one second
            yield return new WaitForSeconds(1f);

            currentTime--;
        }

        // Call the function when the countdown is finished
        OnCountdownFinished();
    }

    private void OnCountdownFinished() {
        // Your code to execute when the timer reaches 0
        SceneManager.LoadScene("GameOver");
    }
}
