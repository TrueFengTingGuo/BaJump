using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_Weapon : MonoBehaviour
{
    public Animator player_animator;
    public Bot_Controller bot_controller;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log( cancel_all_weapon + " , "+fire + " , " +weapons[current_weapon_index].current_cd  );
        if(!cancel_all_weapon){
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

        }
        else{// cancel all possible weapons
        }

        //calculate all weapons cd time
        for(int count = 0; count < total_weapon; count++){
            if(weapons[current_weapon_index].current_cd >= 0){
                weapons[current_weapon_index].current_cd -= Time.deltaTime;
            }
        }
    }

    public void switch_Weapon(){
        
        current_weapon_index = (current_weapon_index + 1) % total_weapon;
        weapons[current_weapon_index].reset();
        player_animator.SetBool(weapons[current_weapon_index].first_animation_name, false);
        if(weapons[current_weapon_index].secondary_animation_name != ""){
            player_animator.SetBool(weapons[current_weapon_index].secondary_animation_name, false);
        }
        Debug.Log("Switch, current_weapon_index: " + current_weapon_index);
        fire = false;
    }

    private void laser_Attack(){

        CharacterInformation attackTargetInfo; 
        
        if((weapons[current_weapon_index].current_loading_time == weapons[current_weapon_index].loading_time) && (weapons[current_weapon_index].current_shooting_time == weapons[current_weapon_index].shooting_time) ){
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, true);
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
                bot_controller.setImpulse(weapons[current_weapon_index].recoil_duration,weapons[current_weapon_index].recoil_power,-direction);
            }

            //create a raycast
            hit = Physics2D.Raycast(laser_fire_point.transform.position,direction);

            if(hit.collider != null){
                if( hit.transform.tag == "Character"){
                    
                    attackTargetInfo =  hit.collider.gameObject.GetComponent<PlayerController>().characterInfo;
                    //Debug.Log(hit.collider.gameObject.GetComponent<Bot_Controller>());
                    attackTargetInfo.taking_damage(Time.deltaTime * 20,laser_fire_point.transform.position,hit.collider.gameObject.transform.position); //take damage
                    
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

            //Debug.Log(weapons[current_weapon_index].current_cd + " , " + weapons[current_weapon_index].cd ); 
        }
    }

    public void holding_shield(){
        //Debug.Log("Hold");
        if(fire_buttonHold == true){
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, true);

        }

        //reset when shield attack is finished
        if((!fire_buttonHold)){
            
            player_animator.SetBool(weapons[current_weapon_index].first_animation_name, false);
            fire = false;
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
