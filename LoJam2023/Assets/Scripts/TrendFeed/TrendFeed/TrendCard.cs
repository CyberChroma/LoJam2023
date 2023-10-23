using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events; 

/// <summary>
/// Appears in the TrendFeed as a CollectableObject with a score modifier.
/// </summary>
public class TrendCard: MonoBehaviour
{
    [SerializeField]
    GameObject timerTextObj;

    [SerializeField]
    GameObject nameTextObj;

    [SerializeField]
    GameObject scoreTextObj;
    
    RectTransform cardRect;

    UnityEvent<TrendCard> timeoutEvent;

    //Text objects
    TextMeshProUGUI scoreText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI nameText;

    string objectName;
    float timeRemaining = 0;
    int objectScore;

    //Information about the object being stored.
    public string ObjectName => objectName;
    public int ObjectScore => objectScore;
    public float TimeRemaining => timeRemaining;

    private void Awake()
    {
        timeoutEvent = new UnityEvent<TrendCard>();
        cardRect = GetComponent<RectTransform>();

        if (timerTextObj == null)
            Debug.LogError("Missing TimerText Object");

        if (scoreTextObj == null)
            Debug.LogError("Missing ScoreText Object");

        if (nameTextObj != null)
            nameText = nameTextObj.GetComponent<TextMeshProUGUI>();

        scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
        timerText = timerTextObj.GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        cardRect.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Activate the TrendCard, supplying its information to be displayed.
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="objectScore"></param>
    /// <param name="lifetime"></param>
    /// <param name="timeoutAction"></param>
    public void Activate(string objectName, int objectScore, int lifetime, UnityAction<TrendCard> timeoutAction)
    {
        this.objectName = objectName;
        this.objectScore = objectScore;
        scoreText.text = objectScore.ToString();
        timeRemaining = lifetime;
        timeoutEvent.AddListener(timeoutAction);

        if (nameText)
            nameText.text = objectName;
    }

    /// <summary>
    /// Force deactivate the Trend Card, removing it from the feed.
    /// </summary>
    public void Deactivate()
    {
        timeoutEvent.RemoveAllListeners();
        timeRemaining = 0;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the time remaining display
    /// </summary>
    void DisplayTime()
    {
        float minutesLeft = Mathf.FloorToInt(timeRemaining / 60);
        float secondsLeft = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutesLeft, secondsLeft);
    }

    /// <summary>
    /// Initialization step. Set the desired width for the GameObject RectTransform.
    /// </summary>
    /// <param name="width"></param>
    public void SetCardWidth(int width)
    {
        cardRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        cardRect.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Update the time remaining for the TrendCard according to the change received.
    /// </summary>
    /// <param name="timeChange"></param>
    public void UpdateTimeRemaining(float timeChange)
    {
        timeRemaining -= timeChange;
        DisplayTime();

        if (timeRemaining <= 0)
        {
            timeoutEvent.Invoke(this);
        }
    }

    /// <summary>
    /// Update this card's time remaining.
    /// </summary>
    private void Update()
    {
        UpdateTimeRemaining(Time.deltaTime);
    }
}
