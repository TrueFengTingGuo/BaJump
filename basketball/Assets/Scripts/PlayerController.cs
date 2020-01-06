using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
[RequireComponent(typeof(Player_Weapon))]
public class PlayerController : MonoBehaviour
{

    public float speed = 8;
    public float acceleration = 12;
    public float gravity = 20;
    public float jumpHeightCurrent = 20;
    public float weightedJump = 13;
    public float lessWeightJump = 20;
    public float shootPower = 20;

    public GameObject ballTag;

    private float currentSpeed;
    private float targetSpeed;
    private Vector2 amountToMove;
    public float shootSpeed;
    private bool powerUpThrow = false;

    [HideInInspector]
    private PlayerPhysics playerPhysics;
    public GameMan gameManagor;
    private SpriteRenderer mySpriteRenderer;

    //impulse after a score
    private Vector2 impulseDircetion;
    private float impulseDuration = 0;
    private float impulsePower = 0;

    public Animator player_animator;
    public BallController ballController;

    //weapon system
    public Player_Weapon weaponSyetem;

    //character information
    public CharacterInformation characterInfo;

    //update info to player info bar
    public Player_ingame_info_bar status_bar;

    // Start is called before the first frame update
    void Start()
    {
        playerPhysics = GetComponent<PlayerPhysics>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        gameManagor = GameObject.Find("Main Camera").GetComponent<GameMan>();
        characterInfo = new CharacterInformation(10,2,100); 
    }

    // Update is called once per frame
    void Update()
    {
        //update playerinfo
        characterInfo.info_update();
        status_bar.changeHealth((characterInfo.current_hp/characterInfo.hp));
        status_bar.changeShield((characterInfo.currentPersonalShield/characterInfo.personalShield));
        status_bar.changeThrow((shootSpeed/50f));


        if(characterInfo.died){
            targetSpeed = 0;
            currentSpeed = 0;
            player_animator.SetBool("died", true);
        }
        else{
            player_animator.SetBool("died", false);
            if (playerPhysics.movementStopped)
            {
                targetSpeed = 0;
                currentSpeed = 0;
            }

            targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
            currentSpeed = incrementTowards(currentSpeed, targetSpeed, acceleration);
            player_animator.SetFloat("Run", Mathf.Abs(currentSpeed));
        
            //rotation
            if (currentSpeed < 0)
            {
                if (mySpriteRenderer != null)
                {
                    // flip the sprite
                    mySpriteRenderer.flipX = true;
                    weaponSyetem.fire_point_flip(true);
                    characterInfo.change_facing_direction(true); // record charater facing info

                }
            }
            else if (currentSpeed > 0)
            {
                if (mySpriteRenderer != null)
                { 
                    // flip the sprite
                    mySpriteRenderer.flipX = false;
                    weaponSyetem.fire_point_flip(false);
                    characterInfo.change_facing_direction(false);// record charater facing info
                }
            }

            //Jump
            if (playerPhysics.grounded)
            {
                
                amountToMove.y = 0;
                if (Input.GetButtonDown("Jump"))
                {
                    amountToMove.y = jumpHeightCurrent;
                    player_animator.SetBool("Is_Jump", true);

                    //cancel all active weapon
                }
                else
                {
                    player_animator.SetBool("Is_Jump", false);
                }

            }
        }
        


        //hold the ball
        if (playerPhysics.holdBall)
        {
            weaponSyetem.force_to_Stop_Weapon();// stop all weapons. 
            jumpHeightCurrent = weightedJump;
            ballTag.GetComponent<Renderer>().enabled = true;

            if (Input.GetMouseButtonDown(0))
            {
                powerUpThrow = true;

            }
            else if (((Input.GetMouseButtonUp(0)) && shootSpeed != 0)||characterInfo.hurted )
            {
                playerPhysics.holdBall = false;
                powerUpThrow = false;

                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (Vector2)((worldMousePos - transform.position));

                //if player is hurt, force to throw
                if(characterInfo.hurted){
                    direction = new Vector2( 0f,1f);
                }
                direction.Normalize();

                //throw ball
                gameManagor.inGameBall = Instantiate(gameManagor.ballPrefab, transform.position + (Vector3)(direction * 0.5f), Quaternion.identity);
                gameManagor.ballExist = true;
                gameManagor.ballHolder = this.gameObject;


                if (playerPhysics.acquireCooldown <= 0f)
                {
                    playerPhysics.acquireCooldown = 1.5f;
                }


                if (gameManagor.inGameBall != null)
                {
                    GameObject ball = gameManagor?.inGameBall;

                    //if player is hurt,using different force
                    if(characterInfo.hurted){
                        ball.GetComponent<Rigidbody2D>().AddForce(direction * 30, ForceMode2D.Impulse);
                    }
                    ball.GetComponent<Rigidbody2D>().AddForce(direction * shootSpeed, ForceMode2D.Impulse);
                    ball?.GetComponent<BallController>()?.shoot(this.gameObject);
                }
                //reset
                shootSpeed = 0f;

            }
        }
        else
        {
            jumpHeightCurrent = lessWeightJump;
            ballTag.GetComponent<Renderer>().enabled = false;
            //attack
            if (Input.GetMouseButtonDown(0))
            {
                if(!weaponSyetem.fire){ // if laser is still firing
                    weaponSyetem.fire = true;
                    weaponSyetem.fire_buttonHold = true;
                }
                
            }
            else if(Input.GetMouseButtonDown(1)){
                weaponSyetem.switch_Weapon(); //switch weapon
            }
            else if(Input.GetMouseButtonUp(0)){
                weaponSyetem.fire_buttonHold = false;
                
            }

        }

        //hold the space key will gain more power when throw the ball
        if (powerUpThrow)
        {
            shootSpeed += shootPower * Time.deltaTime;
            if (shootSpeed > 50f)
            {
                shootSpeed = 50f;
            }

        }

        //gravity
        amountToMove.x = currentSpeed;
        amountToMove.y -= gravity * Time.deltaTime;

        //impulse after a scroe
        if(impulseDuration > 0){
            amountToMove.x += impulseDircetion.x * impulsePower; 
            impulseDuration -= Time.deltaTime;
        }
        
        
        //move
        playerPhysics.Move(amountToMove * Time.deltaTime);
    }


    //increase the target by speed
    private float incrementTowards(float n, float target, float localAcceleration)
    { //increase n towards target by speed
        if (n == target)
        {

            return n;
        }
        else
        {
            float dir = Mathf.Sign(target - n); // n must be increased or decrease ti get closer to target
            n += localAcceleration * Time.deltaTime * dir;
            return (dir == Mathf.Sign(target - n)) ? n : target; //if n has now passed target thenreturn target, otherwise return m
        }
    }

    //give a force to player
    public void setImpulse(float duration, float power, Vector2 direction){

        impulseDircetion = direction;
        impulseDuration = duration;
        impulsePower = power;

    }

}