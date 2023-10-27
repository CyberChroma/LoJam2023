using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    private float totalTimeInSeconds = 300f;

    void Start() {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown() {
        float currentTime = totalTimeInSeconds;

        while (currentTime >= 0) {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        OnCountdownFinished();
    }

    private void OnCountdownFinished() {
        SceneManager.LoadScene("GameOver");
    }
}
