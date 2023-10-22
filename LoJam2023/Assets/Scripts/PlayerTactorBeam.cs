using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
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
        activePullObject.GetComponent<TrendObject>().isDisappearing = true;
        activePullObject.GetComponent<Collider2D>().enabled = false;
        activePullObject = null;
        yield return new WaitForSeconds(1f);
        Destroy(activePullObject);
    }
}
