using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CircleCollider2D))]
public class BallPhyscis : MonoBehaviour {

    private Rigidbody2D rb2D;

    [HideInInspector]
    public bool acquired;
    private CircleCollider2D collider;

    void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D> ();
        acquired = false;

    }

    public void Move (Vector2 force) {

        rb2D.AddForce (force, ForceMode2D.Force);

    }
}