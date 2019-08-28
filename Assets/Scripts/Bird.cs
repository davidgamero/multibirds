using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour{

	private int points;

	public static int moveSpeed = 50000;
	public static int normalSpeed = 50000;
	public static int hellSpeed = 150000;
	public static int jumpHeight = 48000;
		//48000;

	private float topRotBound = 25f;
	private float botRotBound = 270f;

	private float dropTime = 0.4f;
	private float speedDropTime = 0.6f;
	private float freeFallAdd = -80f;
	private bool freeFalling = false;

	private float _sinceJump = 0f;

	private GameObject mainCamera;
	private GameObject ground;

	private bool alive = true;	//keep track of whether game is going or you're dead

	//sound
	public static AudioClip flap;
	public static AudioClip smack;
	public static AudioClip drop;
	public static AudioClip point;

	// Use this for initialization
	void Start () {
		points = 0;

		mainCamera = GameObject.Find ("Main Camera");
		ground = GameObject.Find ("Ground");

		flap = (AudioClip)Resources.Load("flap");
		smack = (AudioClip)Resources.Load("smack");
		drop = (AudioClip)Resources.Load("drop");
		point = (AudioClip)Resources.Load("point");

		if (PlayerPrefs.GetInt ("hellmode", 0) == 1) {
			moveSpeed = hellSpeed;
		}else{
			moveSpeed = normalSpeed;
		};
	}

	//called once per frame
	void Update() {
		
	}

	// FixedUpdate is called once per frame
	void FixedUpdate () {

		GoRight();

		_sinceJump += Time.deltaTime;

		//freefall stuff
		if(alive){

			//if it's time to freefall
			if (_sinceJump > dropTime) {
				gameObject.rigidbody2D.AddTorque (-15000f * Time.deltaTime);

			}else{

			}

			if (_sinceJump > speedDropTime) {
				freeFalling = true;
				gameObject.rigidbody2D.velocity = new Vector3(gameObject.rigidbody2D.velocity.x,
				                                              gameObject.rigidbody2D.velocity.y + freeFallAdd * Time.deltaTime);

			}else{
				freeFalling = false;
			}

		}

		float nZRot = gameObject.transform.eulerAngles.z;
		
		//Clamp z Rot Value
		if((gameObject.transform.eulerAngles.z > topRotBound) && (gameObject.transform.eulerAngles.z < ((topRotBound + botRotBound) / 2.0f))){
			nZRot = topRotBound;
			gameObject.rigidbody2D.angularVelocity = 0;
		}
		if((gameObject.transform.eulerAngles.z < botRotBound) && (gameObject.transform.eulerAngles.z > ((topRotBound + botRotBound) / 2.0f))){
			nZRot = botRotBound;
			gameObject.rigidbody2D.angularVelocity = 0;
		}
		
		gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x,
		                                               gameObject.transform.eulerAngles.y,
		                                               nZRot);
		
	}

	//2D collision wiht normal object
	void OnCollisionEnter2D(Collision2D collision2D){
		//If ran into pipe
		if (collision2D.collider.gameObject.layer == LayerMask.NameToLayer ("Tubes")) {
			RanIntoPipe();
		}
		//If ran into ground
		if (collision2D.collider.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			RanIntoGround();
		}

	}

	void OnTriggerEnter2D(Collider2D collider2D){
		//If ran into score adder
		if (collider2D.gameObject.tag == "Scorer") {
			score();
			print("got a point. now has " + points.ToString());
		}
	}

	//called when bird first collides with pipe
	void RanIntoPipe(){
		alive = false;

		PlaySmackSound();

		collider2D.enabled = false;

		print ("ran into pipe");
	}

	void RanIntoGround(){
		alive = false;

		PlayDropSound();

		//make it freeze stuck into the ground
		collider2D.enabled = false;
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.gravityScale = 0f;
		gameObject.BroadcastMessage ("HitGround");


		print ("ran into ground");
	}

	//apply up force to jump the bird
	public void Jump(){
			if (alive) {
				PlayFlapSound();
				//actually go up on jump if alive
				gameObject.rigidbody2D.velocity = new Vector3 (gameObject.rigidbody2D.velocity.x, 0, 0);
				gameObject.rigidbody2D.AddForce (new Vector2 (0, jumpHeight));
				_sinceJump = 0f;
				gameObject.rigidbody2D.AddTorque (10000f);
			}
	}
	//function to keep applying force to move bird right
	public void GoRight(){
		//if the game is happening keep going right
		if (alive) {
			gameObject.rigidbody2D.AddForce (new Vector2 (moveSpeed * Time.deltaTime, 0));
		}
	}

	//called when object first collides with scorer tagged object
 	public void score(){
		PlayPointSound();
		points += 1;
	}

	//accessor for score
	public int getScore(){
		return points;
	}

	//accessor for bool Alive
	public bool getAlive(){
		return alive;
	}
	//sound functions
	void PlayFlapSound(){
		audio.PlayOneShot (flap);
	}
	void PlaySmackSound(){
		audio.PlayOneShot (smack);
	}
	void PlayDropSound(){
		audio.PlayOneShot (drop);
	}
	void PlayPointSound(){
		audio.PlayOneShot (point);
	}

}
