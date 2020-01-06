using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallPhyscis))]
public class BallController : MonoBehaviour
{
    [HideInInspector]
    public BallPhyscis ballPhyscis;
    public GameObject shootingPlayer;

    private float ignoreCollisionTime = 0;
    private Vector2 force;

    // Start is called before the first frame update
    void Start()
    {
        ballPhyscis = GetComponent<BallPhyscis>();
    }

    void Update()
    {

        if (shootingPlayer != null && ignoreCollisionTime > 0)
        {
            ignoreCollisionTime -= Time.deltaTime;
        }

        if (shootingPlayer != null && ignoreCollisionTime < 0)
        {
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), shootingPlayer.GetComponent<BoxCollider2D>(), false);
            shootingPlayer = null;
        }


    }
    public void shoot(GameObject player)
    {
        ignoreCollisionTime = 1;
        shootingPlayer = player;
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), shootingPlayer.GetComponent<BoxCollider2D>(), true);
    }
}