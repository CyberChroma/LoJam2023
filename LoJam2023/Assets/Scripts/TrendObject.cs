using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrendObject : MonoBehaviour
{
    public int points = 10;

    [HideInInspector] public bool isDisappearing;

    private Rigidbody2D rb;
    private PlayerTactorBeam playerTactorBeam;
    private PlayerMove playerMove;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTactorBeam = FindObjectOfType<PlayerTactorBeam>(true);
        playerMove = playerTactorBeam.GetComponentInParent<PlayerMove>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDisappearing) {
            Vector3 direction = playerTactorBeam.transform.position - transform.position;
            rb.angularVelocity = -playerTactorBeam.spinSpeed * 2;
            if (rb.position.y >= playerTactorBeam.transform.position.y) {
                rb.velocity = Vector2.zero;
                rb.position = playerTactorBeam.transform.position;
            } else {
                rb.velocity = direction * playerTactorBeam.disappearPullVelocity;
                rb.velocity = new Vector2(rb.velocity.x * 20, Mathf.Abs(rb.velocity.y) * 2 + Mathf.Abs(playerMove.rb.velocity.y));
            }
            transform.localScale *= 0.95f;
            rb.gravityScale = 0;
        } else {
            if (playerTactorBeam.activePullObject == gameObject) {
                Vector3 direction = playerTactorBeam.activePosition.position - transform.position;
                rb.angularVelocity = -playerTactorBeam.spinSpeed;
                rb.AddForce(direction * playerTactorBeam.pullSpeed);
                rb.gravityScale = 0;
            } else {
                rb.gravityScale = 15;
            }
        }
    }
}
