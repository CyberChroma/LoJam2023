using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMove : MonoBehaviour {
    public float speed = 1.0f;
    public float activeTime = 50;

    private RectTransform rectTransform;
    private bool active;

    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {
        if (active) {
            Vector3 currentPosition = rectTransform.anchoredPosition;
            float newYPosition = currentPosition.y + (speed * Time.deltaTime);
            Vector3 newPosition = new Vector3(currentPosition.x, newYPosition, currentPosition.z);
            rectTransform.anchoredPosition = newPosition;
        }
    }

    public void Activate() {
        active = true;
        StartCoroutine(WaitToDeactivate());
    }

    IEnumerator WaitToDeactivate() {
        yield return new WaitForSeconds(activeTime);
        active = false;
    }
}