using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour {
    public float hoverHeight = 10.0f;
    public float hoverSpeed = 1.0f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private float timeOffset;

    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        timeOffset = Random.Range(0.0f, 2.0f * Mathf.PI);
    }

    // Update is called once per frame
    void Update() {
        float newYPosition = originalPosition.y + Mathf.Sin((Time.time + timeOffset) * hoverSpeed) * hoverHeight;
        Vector3 newPosition = new Vector3(originalPosition.x, newYPosition, originalPosition.z);
        rectTransform.anchoredPosition = newPosition;
    }
}
