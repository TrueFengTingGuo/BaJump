using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class PlayerPhysics : MonoBehaviour {
    public LayerMask collisionMask;
    public LayerMask ballMask;
    private BoxCollider2D collider;
    private Vector2 size;
    private Vector2 offest;

    [HideInInspector]
    public float acquireCooldown = 1;

    [HideInInspector]
    public bool holdBall = false;
    public GameMan gameManagor;

    private float skin = .005f;

    [HideInInspector]
    public bool grounded;

    [HideInInspector]
    public bool movementStopped;

    Ray ray;
    RaycastHit hit;

    void Start () {

        collider = GetComponent<BoxCollider2D> ();
        gameManagor = GameObject.Find ("Main Camera").GetComponent<GameMan> ();
        size = collider.size;
        offest = collider.offset;
        
    }
    public void Move (Vector2 moveAmount) {
        float deltaY = moveAmount.y;
        float deltaX = moveAmount.x;
        float gameobjectScale = transform.localScale.x;

        Vector2 p = transform.position; // player current position

        grounded = false;

        for (int i = 0; i < 3; i++) {

            float dir = Mathf.Sign (deltaY); //up or down
            float x = (p.x + offest.x - size.x / 2 * gameobjectScale) + size.x / 2 * i * gameobjectScale; //Left, centre and then rightmost point of collider
            float y = p.y + offest.y + size.y / 2 * dir * gameobjectScale; //bottom of collider

            ray = new Ray (new Vector2 (x, y), new Vector2 (dir, 0));

            //hit wall////////////
            RaycastHit2D hit = Physics2D.Raycast (new Vector2 (x, y), new Vector2 (0, dir), Mathf.Abs (deltaY) + skin, collisionMask);

            if (hit.collider != null) {

                //Get Distance between player and ground
                float dst = Vector2.Distance (ray.origin, hit.point);
                //Stop player's downwards movement after coming within skin width of a collider
                if (dst > skin) {

                    deltaY = dst * dir - skin * dir;
                } else {
                    deltaY = 0;
                }
                grounded = true;

            }

            //hit ball/////////////
            if (gameManagor.ballExist) {
                hit = Physics2D.Raycast (new Vector2 (x, y), new Vector2 (0, dir), Mathf.Abs (deltaY) + skin, ballMask);
                if (acquireCooldown < 0) {
                    if (hit.collider != null) {
                        //Get Distance between player and ground
                        float dst = Vector2.Distance (ray.origin, hit.point);
                        if (!gameManagor.inGameBall.GetComponent<BallController> ().ballPhyscis.acquired) {
                            holdBall = true;
                            if (gameManagor.ballExist) {
                                Destroy (gameManagor.inGameBall);
                                gameManagor.ballExist = false;
                            }
                        }

                    }
                }
            }

        }

        //check collision left and right
        movementStopped = false;
        for (int i = 0; i < 3; i++) {

            float dir = Mathf.Sign (deltaX); //left and right
            float x = p.x + offest.x + size.x / 2 * dir * gameobjectScale;
            float y = p.y + offest.y - size.y / 2 + size.y / 2 * i * gameobjectScale;

            ray = new Ray (new Vector2 (x, y), new Vector2 (dir, 0));

            //hit the wall
            RaycastHit2D hit = Physics2D.Raycast (new Vector2 (x, y), new Vector2 (dir, 0), Mathf.Abs (deltaX) + skin, collisionMask);
            if (hit.collider != null) {
                //Get Distance between player and ground
                float dst = Vector2.Distance (ray.origin, hit.point);
                //Stop player's downwards movement after coming within skin width of a collider
                if (dst > skin) {
                    deltaX = dst * dir - skin * dir;
                } else {
                    deltaX = 0;
                }
                movementStopped = true;

            }

            if (gameManagor.ballExist) {
                //hit the ball
                hit = Physics2D.Raycast (new Vector2 (x, y), new Vector2 (dir, 0), Mathf.Abs (deltaX) + skin, ballMask);
                if (acquireCooldown < 0) {
                    if (hit.collider != null) {
                        //Get Distance between player and ground
                        //float dst = Vector2.Distance (ray.origin, hit.point);
                        if (!gameManagor.inGameBall.GetComponent<BallController> ().ballPhyscis.acquired) {
                            gameManagor.inGameBall.GetComponent<BallController> ().ballPhyscis.acquired = true;
                            holdBall = true;
                            if (gameManagor.ballExist) {
                                Destroy (gameManagor.inGameBall);
                                gameManagor.ballExist = false;
                            }
                        }

                    }
                }
            }

        }

        if (grounded && !movementStopped) {
            Vector2 playerDirec = new Vector2 (deltaX, deltaY);
            Vector2 origin = new Vector2 (p.x + offest.x + size.x / 2 * Mathf.Sign (deltaX) * gameobjectScale, p.y + offest.y + size.y / 2 * Mathf.Sign (deltaY) * gameobjectScale);

            if (Physics2D.Raycast (origin, playerDirec.normalized, Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY), collisionMask).collider != null) {
                grounded = true;
                deltaY = 0;
            }
        }

        if (acquireCooldown > 0f) {
            acquireCooldown -= 1f * Time.deltaTime;
        } else if (acquireCooldown <= 0f) {

        }
        Vector2 finalTransfrom = new Vector2 (deltaX, deltaY);
        transform.Translate (finalTransfrom);

    }
}