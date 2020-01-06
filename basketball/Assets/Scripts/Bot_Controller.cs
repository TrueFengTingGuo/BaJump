using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
[RequireComponent(typeof(Bot_Weapon))]
public class Bot_Controller : MonoBehaviour
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
    public Bot_Weapon weaponSyetem;

    //characterinfo
    public CharacterInformation characterInfo;

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
        
        //update character info
        characterInfo.info_update();

        //if character is hurt, then character cant be controlled
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
 
            Vector3 ball_current_position = new Vector3();
            if (gameManagor.inGameBall != null)
            {
                GameObject ball = gameManagor?.inGameBall;
                ball_current_position = ball.transform.position;
                if((ball_current_position.x- this.transform.position.x) < 0){
                    targetSpeed = (-1) * speed;
                }
                else{
                    targetSpeed = 1* speed;
                }

            }
            else{//if player has the ball
                GameObject player = gameManagor.player1;
                Vector3 player_current_position = player.transform.position;
                if(player_current_position.x - this.transform.position.x < 0){
                    targetSpeed = (-1) * speed;
                }
                else{
                    targetSpeed = 1* speed;
                }
            }

            
            
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
                }
            }
            else if (currentSpeed > 0)
            {
                if (mySpriteRenderer != null)
                { 
                    // flip the sprite
                    mySpriteRenderer.flipX = false;
                    weaponSyetem.fire_point_flip(false);
                }
            }

            //Jump
            if (playerPhysics.grounded)
            {
                bool jump = false;

                GameObject player = gameManagor.player1;
                Vector3 player_current_position = player.transform.position;
                if(player_current_position.y - this.transform.position.y > 5){
                    jump = true;
                }
                else{
                    jump = false;
                }

                amountToMove.y = 0;
                if (jump)
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
            jumpHeightCurrent = weightedJump;
            ballTag.GetComponent<Renderer>().enabled = true;

            //random throw time
            bool throwBall = false;
            int randomThrow = Random.Range(0, 1000);

            if(randomThrow < 10 || characterInfo.hurted){
                throwBall = true;
            }
            else{
                throwBall = false;
            }

            //bool throw a ball
            if (!throwBall)
            {
                powerUpThrow = true;

            }
            else if (throwBall && shootSpeed != 0)
            {
                playerPhysics.holdBall = false;
                powerUpThrow = false;

                Vector2 direction = new Vector2( Random.Range(-1f, 1f), Random.Range(0f, 1f));
               
                //if player is hurt, force to throw
                if(characterInfo.hurted){
                    direction = new Vector2( 0f,1f);
                }

                direction.Normalize();

                //throw the ball
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

                    //if player is hurt
                    if(characterInfo.hurted){
                        ball.GetComponent<Rigidbody2D>().AddForce(direction * 30, ForceMode2D.Impulse);
                    }
                    else{
                        ball.GetComponent<Rigidbody2D>().AddForce(direction * shootSpeed, ForceMode2D.Impulse);
                    }
                    
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

            //attack player

            if (gameManagor.inGameBall == null)
            {
                bot_attack_player();

                /*if (Input.GetMouseButtonDown(0))
                {
                    if(!weaponSyetem.fire){ // if laser is still firing
                        weaponSyetem.fire = true;
                        weaponSyetem.fire_buttonHold = true;
                    }
                }
                else if(Input.GetMouseButtonDown(1)){
                  weaponSyetem.switch_Weapon();
                }
                else if(Input.GetMouseButtonUp(0)){
                    weaponSyetem.fire_buttonHold = false;
                } */
                
            }

            

        }

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

    public void setImpulse(float duration, float power, Vector2 direction){

        impulseDircetion = direction;
        impulseDuration = duration;
        impulsePower = power;

    }

    private void bot_attack_player(){
        GameObject player = gameManagor.player1;
        Vector3 player_current_position = player.transform.position;
        if(player_current_position.x - this.transform.position.x < 50){
            if(!weaponSyetem.fire){ // if laser is still firing
                weaponSyetem.fire = true;
                weaponSyetem.fire_buttonHold = true;
            }
        }

    }
}
