using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public float minSpawnTime = 1;
    public float maxSpawnTime = 5;
    public GameObject[] trendObjectsArea1;
    public GameObject[] trendObjectsArea2;
    public GameObject[] trendObjectsArea3;

    private LevelSwitcher levelSwitcher;

    // Start is called before the first frame update
    void Start()
    {
        levelSwitcher = FindObjectOfType<LevelSwitcher>();
        StartCoroutine(WaitToSpawn());
    }

    IEnumerator WaitToSpawn() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            SpawnNewTrendObject();
        }
    }

    void SpawnNewTrendObject() {
        int trendObjectChoice;
        GameObject newTrendObject = null;
        switch (levelSwitcher.currentLevel) {
            case 0:
                trendObjectChoice = Random.Range(0, trendObjectsArea1.Length);
                newTrendObject = Instantiate(trendObjectsArea1[trendObjectChoice], transform);
                newTrendObject.name = trendObjectsArea1[trendObjectChoice].name;
                break;
            case 1:
                trendObjectChoice = Random.Range(0, trendObjectsArea2.Length);
                newTrendObject = Instantiate(trendObjectsArea2[trendObjectChoice], transform);
                newTrendObject.name = trendObjectsArea2[trendObjectChoice].name;
                break;
            case 2:
                trendObjectChoice = Random.Range(0, trendObjectsArea3.Length);
                newTrendObject = Instantiate(trendObjectsArea3[trendObjectChoice], transform);
                newTrendObject.name = trendObjectsArea3[trendObjectChoice].name;
                break;
        }

        do {
            newTrendObject.transform.position = new Vector2(Random.Range(-101f, 101f), 10f);
            SnapObjectToFloor(newTrendObject);
        } while (IsPositionInView(newTrendObject.transform.position));
    }

    bool IsPositionInView(Vector2 worldPosition) {
        // Convert the world position to viewport position
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);

        // Check if it's inside the viewport
        if (viewportPosition.x >= -0.1f && viewportPosition.x <= 1.1f &&
            viewportPosition.y >= -0.1f && viewportPosition.y <= 1.1f) {
            return true;
        } else {
            return false;
        }
    }

    void SnapObjectToFloor(GameObject newTrendObject) {
        // Cast a ray downwards from the object's position
        RaycastHit2D hit = Physics2D.Raycast(newTrendObject.transform.position, Vector2.down, Mathf.Infinity);

        // If the ray hits the floor, set the object's position to the hit point
        if (hit.collider != null) {
            Vector2 floorContactPoint = hit.point;
            newTrendObject.transform.position = new Vector3(floorContactPoint.x, floorContactPoint.y + 0.5f, newTrendObject.transform.position.z);
        }
    }

    public void ClearObjects() {
        foreach (Transform childTransform in transform) {
            Destroy(childTransform.gameObject);
        }
    }
}
