using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    public Transform target;
    private float trackSpeed = 10;

    public void setTarget (Transform t) {
        target = t;
    }

    //update after Update()
    void LateUpdate () {
        if (target) {
            float x = incrementTowards(transform.position.x,target.position.x,trackSpeed);
            float y = incrementTowards(transform.position.y,target.position.y,trackSpeed);
            transform.position= new Vector3(x,y,transform.position.z); 
        }
    }

    //increase the target by speed
    private float incrementTowards (float n, float target, float localAcceleration) { //increase n towards target by speed
        if (n == target) {

            return n;
        } else {
            float dir = Mathf.Sign (target - n); // n must be increased or decrease ti get closer to target
            n += localAcceleration * Time.deltaTime * dir;
            return (dir == Mathf.Sign (target - n)) ? n : target; //if n has now passed target thenreturn target, otherwise return m
        }
    }
}