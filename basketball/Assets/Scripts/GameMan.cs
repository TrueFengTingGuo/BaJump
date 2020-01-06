using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMan : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject botPrefab;
    public GameObject ballPrefab;
    private GameCamera cam;

    [HideInInspector]
    public bool ballExist = false;
    public GameObject ballHolder;
    public GameObject inGameBall;
    public GameObject player1;
    public GameObject player2;

    // Start is called before the first frame update
    void Start () {
        cam = GetComponent<GameCamera> ();
        SpawnPlayer ();
        SpawnBall();

    }

    private void SpawnPlayer () {
        player1 = Instantiate (playerPrefab, Vector3.zero, Quaternion.identity);
        cam.setTarget ( (player1 as GameObject).transform);

        player2 = Instantiate (botPrefab, Vector3.zero, Quaternion.identity);
    }

    private void SpawnBall () {
        inGameBall = Instantiate (ballPrefab, Vector3.zero, Quaternion.identity);
        ballExist = true;
    }
}