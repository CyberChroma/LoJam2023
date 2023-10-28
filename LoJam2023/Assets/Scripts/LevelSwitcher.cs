using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcher : MonoBehaviour
{
    public GameObject[] levels;
    public Transform cameraOffset;
    [HideInInspector] public int currentLevel;

    private CountdownTimer countdownTimer;
    private RandomObjectSpawner randomObjectSpawner;
    private PlayerMove playerMove;
    private PlayerSwing playerSwing;
    private CameraFollow cameraFollow;
    private TrendFeed trendFeed;

    // Start is called before the first frame update
    void Start()
    {
        countdownTimer = FindObjectOfType<CountdownTimer>();
        randomObjectSpawner = FindObjectOfType<RandomObjectSpawner>();
        playerMove = FindObjectOfType<PlayerMove>();
        playerSwing = FindObjectOfType<PlayerSwing>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        trendFeed = FindObjectOfType<TrendFeed>();
        for (int i = 0; i < levels.Length; ++i) {
            levels[i].SetActive(false);
        }
        levels[0].SetActive(true);
    }

    public void SwitchToLevel(int nextLevel) {
        if (nextLevel >= 1) {
            StartCoroutine(LevelTransition(nextLevel - 1));
        }
    }

    IEnumerator LevelTransition(int nextLevelIndex) {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GrowPlayer(Vector3.one * 50));

        for (int i = 0; i < levels.Length; ++i) {
            levels[i].SetActive(i == nextLevelIndex);
        }
        countdownTimer.currentTime += 60;
        randomObjectSpawner.ClearObjects();
        currentLevel = nextLevelIndex;
        trendFeed.SetTrendObjectList(nextLevelIndex);
        trendFeed.ClearAllTrendItems();
        StartCoroutine(ResetPlayer());
    }

    IEnumerator GrowPlayer(Vector3 targetScale) {
        cameraOffset.localPosition = Vector2.zero;
        playerMove.enabled = false;
        float growthRate = 100f;
        float tolerance = 0.01f;

        while (Vector3.Distance(playerSwing.transform.localScale, targetScale) > tolerance) {
            playerSwing.transform.localScale = Vector3.MoveTowards(playerSwing.transform.localScale, targetScale, growthRate * Time.deltaTime);
            yield return null;
        }

        playerSwing.transform.localScale = targetScale;
    }

    IEnumerator ResetPlayer() {
        yield return null;
        playerMove.enabled = true;
        Camera.main.fieldOfView = 2;
        cameraOffset.localPosition = new Vector2(0, 0);
        playerSwing.transform.localScale = Vector3.one;
        cameraFollow.Snap();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ZoomCamera());
    }

    IEnumerator ZoomCamera() {
        float initialFOV = 2;
        float targetFOV = 60;
        float speed = 5f;
        float tolerance = 1f;

        Camera.main.fieldOfView = initialFOV;
        cameraOffset.localPosition = new Vector2(0, -1.5f);

        while (Mathf.Abs(targetFOV - Camera.main.fieldOfView) > tolerance) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, speed * Time.deltaTime);
            yield return null;
        }

        Camera.main.fieldOfView = targetFOV;
        yield return null;
    }
}
