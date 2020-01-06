using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollection
{

    public string first_animation_name;
    public string secondary_animation_name;
    public Vector2 fire_point_position;
    public float loading_time = 0;
    public float shooting_time = 0;
    public float current_loading_time = 0f;
    public float current_shooting_time = 0f;
    public float recoil_duration = 0.4f;
    public float recoil_power = 5f;
    public bool recoil_bool = true;
    public string weaponType;
    public float current_cd;
    public float cd;
    public float damage_one_shot;
    public float damage_per_sec;

    public WeaponCollection(string animation_triggler_name,  string secondary_animation_triggler, float fire_point_x, float fire_point_y, float load_set, float shoot_set
    , float recoil_duration_set, float recoil_power_set, int weapon_type_int,float cd_set){

        first_animation_name = animation_triggler_name;
        secondary_animation_name = secondary_animation_triggler;
        fire_point_position = new Vector2(fire_point_x, fire_point_y);
        loading_time = load_set;
        shooting_time = shoot_set;
        recoil_duration = recoil_duration_set;
        recoil_power = recoil_power_set;

        if(weapon_type_int == 1){
            weaponType = "Laser";
            Debug.Log("laser");
        }
        else if(weapon_type_int == 2){
            weaponType = "Throwable";
        }
        else if(weapon_type_int == 3){
            weaponType = "HoldingObject";
        }

        cd = cd_set;
        current_cd = 0;

    }
    // Start is called before the first frame update
    void Start()
    {
        current_loading_time = loading_time;
        current_shooting_time = shooting_time;
        recoil_bool = true;
    }

    public void reset(){

        current_loading_time = loading_time;
        current_shooting_time = shooting_time;
        current_cd = cd;
        recoil_bool = true;
    }
}
