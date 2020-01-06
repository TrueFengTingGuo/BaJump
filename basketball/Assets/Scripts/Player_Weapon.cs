using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Weapon : MonoBehaviour
{
    public Animator player_animator;
    public PlayerController player_controller;
    
    //weapon
    public GameObject laser_fire_point;
    public LineRenderer lineRenderer_laser;
    [HideInInspector]
    private WeaponCollection[] weapons;
    public bool fire;
    public bool cancel_all_weapon;
    public int total_weapon;
    public int current_weapon_index;
    public bool fire_buttonHold;

    private bool fire_point_flip_bool = false;


    //information display option
    public Text cooldowninfo;

    // Start is called before the first frame update
    void Start()
    {
        total_weapon = 2;
        current_weapon_index = 0;
        weapons = new WeaponCollection[total_weapon];
        weapons[0] = new WeaponCollection("Laser_Attack","",2,0,0.3f,0.3f,0.5f,10,1,3);
        weapons[1] = new WeaponCollection("inital_block_attack","holding_shield",2,0,0.3f,0.3f,0.5f,10,3,1);
        cancel_all_weapon = false;
        fire = false;
        fire_buttonHold = false;
        cooldowninfo = GameObject.Find("Weapon cooldown").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldowninfo.text = weapons[current_weapon_index].current_cd.ToString();

        //Debug.Log( cancel_all_weapon + " , "+fire + " , " +weapons[current_weapon_index].current_cd  );
            //Debug.Log( fire + "" +weapons[current_weapon_index].current_cd  );
        if((fire) && (weapons[current_weapon_index].current_cd <= 0))
        {
                
            if(weapons[current_weapon_index].weaponType == "Laser"){
                laser_Attack();
            }
            else if(weapons[current_weapon_index].weaponType == "Throwable"){
                    
            }
            else if(weapons[current_weapon_index].weaponType == "HoldingObject"){
                   holding_shield();
            }
                
        }
         else{
              fire = false;
        }


        //calculate all weapons cd time
        for(int count = 0; count < total_weapon; count++){
            if(weapons[current_weapon_index].current_cd >= 0){
                weapons[current_weapon_index].current_cd -= Time.deltaTime;
            }
        }
    }

    public void switch_Weapon(){
        
        weapons[current_weapon_index].reset();
        player_animator.SetBool(weapons[current_weapon_index].first_animation_name, false);
        if(weapons[current_weapon_index].secondary_animation_name != ""){
            player_animator.SetBool(weapons[current_weapon_index].secondary_animation_name, false);
        }
        Debug.Log("Switch, current_weapon_index: " + current_weapon_index);
        fire = false;
        current_weapon_index = (current_weapon_index + 1) % total_weapon;
    }

    public void force_to_Stop_Weapon(){
        
        weapons[current_weapon_index].reset();
        player_animator.SetBool(weapons[current_weapon_index].first_animation_name, false);
        if(weapons[current_weapon_index].secondary_animation_name != ""){
            player_animator.SetBool(weapons[current_weapon_index].secondary_animation_name, false);
        }
        fire = false;

    }

    private void laser_Attack(){

        
        if((weapons[current_weapon_index].current_loading_time == weapons[current_weapon_index].loading_time) && (weapons[current_weapon_index].current_shooting_time == weapons[current_weapon_index].shooting_time) ){
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, true);
            player_controller.characterInfo.currentPersonalShield -= 20;
        }
        
       if(weapons[current_weapon_index].current_loading_time >0){
           weapons[current_weapon_index].current_loading_time -= Time.deltaTime;
        }
        else{
            weapons[current_weapon_index].current_shooting_time -= Time.deltaTime;

            RaycastHit2D  hit;
            Vector2 direction;

            //change laser fire direction
            if(fire_point_flip_bool){
                direction = new Vector2(-1,0);
            }
            else{
                direction = new Vector2(1,0);
            }

            //fire the laser 
            //set the starting point of the weapon
            lineRenderer_laser.SetPosition(0, laser_fire_point.transform.position);
            
            //apply recoli
            if(weapons[current_weapon_index].recoil_bool){
                weapons[current_weapon_index].recoil_bool = false;
                player_controller.setImpulse(weapons[current_weapon_index].recoil_duration,weapons[current_weapon_index].recoil_power,-direction);
            }

            //create a raycast
            hit = Physics2D.Raycast(laser_fire_point.transform.position,direction);
            if(hit.collider != null){
                
                //other player takes damage
                if( hit.transform.tag == "Character"){

                    //Debug.Log(hit.collider.gameObject.GetComponent<Bot_Controller>());
                    hit.collider.gameObject.GetComponent<Bot_Controller>().characterInfo.current_hp -= Time.deltaTime * 20; //take damage
                    
                }
                lineRenderer_laser.SetPosition(1,hit.point);
            }
            else{
                lineRenderer_laser.SetPosition(1,direction*500);
            }

       }
        //reset when laser attack is finished
        if((weapons[current_weapon_index].current_loading_time <= 0 ) && (weapons[current_weapon_index].current_shooting_time <= 0)){
            
            lineRenderer_laser.SetPosition(1, laser_fire_point.transform.position);
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, false);
            fire = false;
            weapons[current_weapon_index].reset();

        }
    }

    public void holding_shield(){

        if(fire_buttonHold == true){
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, true);
            player_controller.characterInfo.shield_up = true;
        }

        //reset when shield attack is finished
        if((!fire_buttonHold)){
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, false);
            fire = false;
            player_controller.characterInfo.shield_up = false;
            weapons[current_weapon_index].reset();
        }

    }

    public void fire_point_flip(bool filp){
        
        Vector3 convertTo3 =  weapons[current_weapon_index].fire_point_position;
        fire_point_flip_bool = filp;
        if(filp){
            laser_fire_point.transform.position = this.gameObject.transform.position - convertTo3;
            
        }
        else{
            laser_fire_point.transform.position = this.gameObject.transform.position + convertTo3;
        }

    }

}
