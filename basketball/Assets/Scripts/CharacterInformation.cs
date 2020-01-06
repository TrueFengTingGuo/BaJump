using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInformation
{
    public float hp;
    public float current_hp;
    private float recent_hp; //use for determine if player is hurted
    public float personalShield;
    public float currentPersonalShield;
    public bool hurted = false;
    public bool died = false;
    public float respawn_cooldown;
    public float current_respawn_cooldown;
    public bool respawn = false;
    public bool shield_up;
    public Vector2 facing_direction;



    
    public CharacterInformation(float init_hp, float init_respawn_cooldown,float init_personalShield){
        hp = init_hp;
        current_hp = hp;
        recent_hp = current_hp;
        respawn_cooldown = init_respawn_cooldown;
        current_respawn_cooldown = init_respawn_cooldown;
        personalShield = init_personalShield;
        currentPersonalShield = 0;
        shield_up = false;
    }

    public void info_update()
    {

        //hp
        if (recent_hp != current_hp){
            hurted = true;
        }
        else{
            hurted = false;
        }
        recent_hp = current_hp;
        
        if(current_hp < 0){
            died = true;
            if(respawn && died){
                current_hp = hp;
                died = false;
                respawn = false;
            }
            else if(!respawn && died){ // count down for respawn time
                current_respawn_cooldown -= Time.deltaTime;
                if(current_respawn_cooldown < 0){
                    respawn = true;
                    current_respawn_cooldown = respawn_cooldown;
                }
            }
            
        }

        //shield
        if(currentPersonalShield < 100){
            currentPersonalShield += 0.1f;
        }
        else if (currentPersonalShield > 100){
            currentPersonalShield = 100;
        }
        else if(currentPersonalShield < 0){
            currentPersonalShield = 0;
        }
    }
    
    public void change_facing_direction(bool changeSign){
        if(changeSign){
            facing_direction = new Vector2(-1,0);
        }
        else{
            facing_direction = new Vector2(1,0);
        }
    }

    public void taking_damage(float damage, Vector2 damage_source_location, Vector2 self_location){

        Vector2 direction_of_damage = damage_source_location - self_location;

        //normalize all vectors
        direction_of_damage.Normalize();       

        if(shield_up && personalShield > 0 && ((direction_of_damage.x / facing_direction.x) > 0)){
            personalShield += damage;
        }
        else{
            current_hp -= damage;
        }
    }
}
