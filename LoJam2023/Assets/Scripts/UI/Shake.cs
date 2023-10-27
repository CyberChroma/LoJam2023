using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    public float shakeAngle = 15.0f;
    public float shakeSpeed = 2.0f;

    private RectTransform rectTransform;
    private Quaternion initialRotation;
    private float timeOffset;

    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        initialRotation = rectTransform.rotation;
        timeOffset = Random.Range(0.0f, 2.0f * Mathf.PI);
    }

    // Update is called once per frame
    void Update() {
        float angle = Mathf.Sin((Time.time + timeOffset) * shakeSpeed) * shakeAngle;
        rectTransform.rotation = initialRotation * Quaternion.Euler(0, 0, angle);
    }
}