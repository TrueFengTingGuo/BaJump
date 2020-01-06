using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ingame_info_bar : MonoBehaviour
{
    public Transform healthBar_Scaler;
    public Transform shieldBar_Scaler;
    public Transform throwBar_Scaler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void changeHealth(float healthPencentage){
        healthBar_Scaler.localScale = new Vector3(healthPencentage,1f);
    }

    public void changeShield(float shieldPencentage){
        shieldBar_Scaler.localScale = new Vector3(shieldPencentage,1f);
    }

    public void changeThrow(float throwPencentage){
        throwBar_Scaler.localScale = new Vector3(throwPencentage,1f);
    }
}
