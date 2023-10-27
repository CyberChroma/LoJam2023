using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutFromActivate : MonoBehaviour {
    private bool fading;
    private Image image;

    // Start is called before the first frame update
    void Start() {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        if (fading) {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 1), 10 * Time.deltaTime);
        }
    }

    public void StartFade() {
        StartCoroutine(WaitToFade());
    }

    IEnumerator WaitToFade() {
        yield return new WaitForSeconds(4.5f);
        fading = true;
    }
}