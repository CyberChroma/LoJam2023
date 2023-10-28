using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class ScoreEvent : UnityEvent<int> { }

public class ScoreManager : MonoBehaviour {
    public float currentScore = 0;
    public float[] levelThresholds;
    public float countSpeed = 50f; // Speed at which the score counts up
    public TextMeshProUGUI scoreOutOfText;

    private float displayedScore = 0; // The score that's currently being displayed
    private int currentLevel = 1;
    private TextMeshProUGUI scoreText;
    private LevelSwitcher levelSwitcher;

    [SerializeField]
    private ScoreEvent scoreUpdateEvent;

    // Start is called before the first frame update
    void Start() {
        scoreText = GetComponent<TextMeshProUGUI>();
        levelSwitcher = FindObjectOfType<LevelSwitcher>();
        scoreText.text = Mathf.FloorToInt(displayedScore).ToString();
        scoreOutOfText.text = "/" + levelThresholds[currentLevel-1];

        if (scoreUpdateEvent == null)
            scoreUpdateEvent = new();
    }

    public void AddScore(float scoreToAdd) {
        currentScore += scoreToAdd;
        scoreUpdateEvent.Invoke((int)scoreToAdd);

        // Check for level threshold crossing
        if (currentLevel <= levelThresholds.Length && currentScore >= levelThresholds[currentLevel - 1]) {
            levelSwitcher.SwitchToLevel(currentLevel + 1);
            scoreOutOfText.text = "/" + levelThresholds[currentLevel];
            currentLevel++;
        }

        // Start counting up the visual score
        StartCoroutine(UpdateScoreVisual());
    }

    IEnumerator UpdateScoreVisual() {

        if (currentScore > displayedScore)
        {
            while (displayedScore < currentScore) {
                // Increment the displayed score
                displayedScore = Mathf.Min(displayedScore + (countSpeed * Time.deltaTime), currentScore);

                // Update the text, converting the float to an int for display
                scoreText.text = Mathf.FloorToInt(displayedScore).ToString();

                yield return null;
            }
        }
        else
        {
            while (displayedScore > currentScore)
            {
                // Increment the displayed score
                displayedScore = Mathf.Max(displayedScore - (countSpeed * Time.deltaTime), currentScore);

                // Update the text, converting the float to an int for display
                scoreText.text = Mathf.FloorToInt(displayedScore).ToString();

                yield return null;
            }
        }
    }
}
