using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10;

    [HideInInspector] public Rigidbody2D rb;
    private PlayerTactorBeam playerTractorBeam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTractorBeam = GetComponentInChildren<PlayerTactorBeam>(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(h, v).normalized;
        if (playerTractorBeam.activePullObject) {
            rb.AddForce(direction * speed / 2, ForceMode2D.Impulse);
        } else {
            rb.AddForce(direction * speed, ForceMode2D.Impulse);
        }
    }
}
