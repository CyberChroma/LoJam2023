using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTactorBeam : MonoBehaviour
{
    public enum TractorState {
        Off,
        LockedOn,
        Pulling
    }

    public float pullSpeed = 500;
    public float spinSpeed = 300;
    public float pullTime = 2;
    public float disappearPullVelocity = 4;

    public TractorState tractorState = TractorState.Off;
    public GameObject activePullObject;
    public Transform activePosition;

    private ScoreManager scoreManager;
    private TrendFeed trendFeed;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        trendFeed = FindObjectOfType<TrendFeed>();
    }

    private void OnDisable() {
        activePullObject = null;
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (gameObject.activeSelf && activePullObject == null && collision.CompareTag("TrendObject")) {
            activePullObject = collision.gameObject;
            StartCoroutine(WaitToPull());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject == activePullObject) {
            activePullObject = null;
            StopAllCoroutines();
        }
    }

    IEnumerator WaitToPull() {
        yield return new WaitForSeconds(pullTime);
        TrendObject trendObject = activePullObject.GetComponent<TrendObject>();
        trendObject.isDisappearing = true;
        activePullObject.GetComponent<Collider2D>().enabled = false;
        int scoreToAdd = 0;
        if (trendFeed.IsObjectTrending(trendObject.name)) {
            scoreToAdd = trendFeed.GetTrendScoreForObject(trendObject.name);
        }
        scoreManager.AddScore(scoreToAdd);
        activePullObject = null;
        Destroy(activePullObject, 1);
    }
}
