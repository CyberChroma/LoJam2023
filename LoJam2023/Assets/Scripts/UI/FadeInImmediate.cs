using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImmediate : MonoBehaviour
{
    private bool fading;
    private Image image;

    // Start is called before the first frame update
    void Awake() {
        image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        StartCoroutine(WaitToFade());
    }

    // Update is called once per frame
    void Update() {
        if (fading) {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 0), 5 * Time.deltaTime);
        }
    }

    IEnumerator WaitToFade() {
        yield return new WaitForSeconds(0.2f);
        fading = true;
    }
}
