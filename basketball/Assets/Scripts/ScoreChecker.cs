using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChecker : MonoBehaviour
{
    [HideInInspector]
    public GameMan gameManagor;

    public GameObject firstCheckPoint;
    public GameObject secondCheckPoint;
    public GameObject implusePoint;
    public ParticleSystem basketScroePartical;
    private bool firstPointReach;
    private bool secondPointReach;
    private bool scored;

    public float impulsePower = 5;
    public float impulseDuration = 3;
    public float impulseRange = 20;
    // Start is called before the first frame update
    void Start()
    {
        gameManagor = GameObject.Find("Main Camera").GetComponent<GameMan>();
        firstPointReach = false;
        secondPointReach =false;
        scored = false;

    }

    // Update is called once per frame
    void Update()
    {
        if((gameManagor.ballExist) && (scored == false)){
            if(!firstPointReach){
                 if(Vector2.Distance(firstCheckPoint.transform.position, gameManagor.inGameBall.transform.position) < 1 ){
                        firstPointReach= true;
                        Debug.Log("first point check!!!!!");
                   }
             } 

            //pass the fist check point, then check for the second one
            if(firstPointReach){
                if(Vector2.Distance(secondCheckPoint.transform.position, gameManagor.inGameBall.transform.position) < 1 ){
                    secondPointReach= true;
                    Debug.Log("second point check!!!!!");
                }
            }

            if((firstPointReach ) && (secondPointReach)){
                scored = true;
                Debug.Log("SCORED!!!!!");
                  
            }
        }


        if(scored){
            //add force to all player (push player back)
                if(gameManagor.player1 != null){
                    Vector2 playerOne_direction = (Vector2)((gameManagor.player1.transform.position - implusePoint.transform.position));
                    playerOne_direction.Normalize();
                    float distance = Vector2.Distance(gameManagor.player1.transform.position,implusePoint.transform.position);
                    gameManagor.player1.GetComponent<PlayerController>().setImpulse(impulseDuration,impulsePower* impulseRange/distance,playerOne_direction);
                    basketScroePartical.Play(true);
                }

                if(gameManagor.player2 != null){
                    Vector2 playerTwo_direction = (Vector2)((gameManagor.player2.transform.position - implusePoint.transform.position));
                    playerTwo_direction.Normalize();
                    float distance = Vector2.Distance(gameManagor.player2.transform.position,implusePoint.transform.position);
                    gameManagor.player2.GetComponent<Bot_Controller>().setImpulse(impulseDuration,impulsePower * impulseRange/distance,playerTwo_direction);
                    Debug.Log("push!!!!!");
                    basketScroePartical.Play(true);
                }
                
                scored = false;
                firstPointReach = false;
                secondPointReach =false;

        }
        

    }
}
