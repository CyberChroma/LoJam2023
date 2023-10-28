using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    GameObject player;

    [SerializeField]
    GameObject projectile;

    [SerializeField]
    int shootRange = 25;

    [SerializeField]
    int shootCooldownLength = 5;

    float shootCooldownTimer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shootCooldownTimer <= 0)
        {
            Vector2 plrDistanceVec = player.transform.position - transform.position;
            float plrDistance = (plrDistanceVec).sqrMagnitude;

            if (plrDistance < shootRange)
            {
                Fire(plrDistanceVec);
                shootCooldownTimer = shootCooldownLength;
            }
        }
        else
        {
            shootCooldownTimer -= Time.deltaTime;
        }
    }

    void Fire(Vector2 plrDistanceVec)
    {
        //Definitely not last-minute code...
        GameObject projectileObj = Instantiate(projectile);
        projectileObj.transform.position = transform.position;
        projectileObj.GetComponent<TankProjectile>().MoveDirection = plrDistanceVec;
        projectileObj.GetComponent<TankProjectile>().IsActive = true;
    }
}
