using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

	public float tap_force = 10;
	public float tilt_vel = 5;
	public Vector3 start_pos = new Vector3((float)-1.5 , 0 ,0);
	
	Rigidbody2D rigid_body;
	Quaternion downward_rotate;
	Quaternion forward_rotate;
    GameManager game;
	
    // Start is called before the first frame update
    void Start(){
        rigid_body = GetComponent<Rigidbody2D>();
        downward_rotate = Quaternion.Euler(0, 0, -30);
        forward_rotate = Quaternion.Euler(0, 0, 30);
        rigid_body.simulated = false;
        game = GameManager.Instance;
    }

    void OnEnable(){
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGamePaused += OnGamePaused;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable(){
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGamePaused -= OnGamePaused;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted(){
        rigid_body.velocity = Vector3.zero;
        rigid_body.simulated = true; // gravity effect
    }

    void OnGamePaused(){
        rigid_body.simulated = false;
    }

    void OnGameOverConfirmed(){
        transform.localPosition = start_pos;
        transform.rotation = Quaternion.identity;
    }

    //Update is called once per frame
    void Update(){
        if (game.GameOver)
            return;

        if (Input.GetMouseButtonDown(0)){
        	transform.rotation = forward_rotate;  
            rigid_body.velocity = Vector3.zero;
        	rigid_body.AddForce(Vector2.up * tap_force, ForceMode2D.Force);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, downward_rotate, tilt_vel * Time.deltaTime); //source value to target value over time
    }

    void OnTriggerEnter2D(Collider2D col){
    	if (col.gameObject.tag == "score_zone"){
    		// increase score
            OnPlayerScored(); // event sent to Game Manager
    	}
    	
    	if (col.gameObject.tag == "dead_zone"){
    		rigid_body.simulated = true;
            OnPlayerDied(); // event sent to Game Manager
            //register a dead event
    		//play sound
    	}
    }
}