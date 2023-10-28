using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectile : MonoBehaviour
{
    [SerializeField]
    ScoreManager scoreManager;

    [SerializeField]
    int penaltyScore = 500;

    [SerializeField]
    int lifetime = 5;

    bool isActive = false;

    [SerializeField]
    int moveSpeed = 5;

    Vector2 moveDirection;

    public Vector2 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

    public bool IsActive { get { return isActive; } set { isActive = value; } }

    float currentActiveTime = 0;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isActive)
            return;

        currentActiveTime += Time.deltaTime;
        transform.position += (moveSpeed * Time.deltaTime) * (Vector3)moveDirection;

        if (currentActiveTime > lifetime)
            Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            scoreManager.AddScore(-penaltyScore);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
