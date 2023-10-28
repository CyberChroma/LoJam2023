using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour {
    public int averageEndThreshold = 100000;
    public int goodEndThreshold = 1000000;
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public Color warningColor2 = Color.yellow;

    [HideInInspector] public float currentTime;

    private TextMeshProUGUI timerText;
    private ScoreManager scoreManager;
    private float totalTimeInSeconds = 180f;

    void Start() {
        timerText = GetComponent<TextMeshProUGUI>();
        scoreManager = FindObjectOfType<ScoreManager>();
        StartCoroutine(StartCountdown());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Minus)) {
            OnCountdownFinished();
        }
    }

    private IEnumerator StartCountdown() {
        currentTime = totalTimeInSeconds;

        while (currentTime >= 0) {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";
            if (currentTime <= 30f) {
                if (currentTime % 2 == 0) {
                    timerText.color = warningColor;
                } else {
                    timerText.color = warningColor2;
                }
            } else if (currentTime <= 60f) {
                timerText.color = warningColor;
            } else {
                timerText.color = normalColor;
            }
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        OnCountdownFinished();
    }

    private void OnCountdownFinished() {
        if (scoreManager.currentScore < averageEndThreshold) {
            SceneManager.LoadScene("GameOverBad");
        } else if (scoreManager.currentScore < goodEndThreshold) {
            SceneManager.LoadScene("GameOverAverage");
        } else {
            SceneManager.LoadScene("GameOverGood");
        }
    }
}
