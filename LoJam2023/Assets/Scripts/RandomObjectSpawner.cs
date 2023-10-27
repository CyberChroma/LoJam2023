using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public float minSpawnTime = 1;
    public float maxSpawnTime = 5;
    public GameObject[] trendObjects;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitToSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitToSpawn() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            SpawnNewTrendObject();
        }
    }

    void SpawnNewTrendObject() {
        int trendObjectChoice = Random.Range(0, trendObjects.Length);
        GameObject newTrendObject = Instantiate(trendObjects[trendObjectChoice], transform);
        newTrendObject.name = trendObjects[trendObjectChoice].name;
        do {
            newTrendObject.transform.position = new Vector2(Random.Range(-50f, 50f), 10f);
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
            newTrendObject.transform.position = new Vector3(floorContactPoint.x, floorContactPoint.y, newTrendObject.transform.position.z);
        }
    }

    public void ClearObjects() {
        foreach (Transform childTransform in transform) {
            Destroy(childTransform.gameObject);
        }
    }
}
