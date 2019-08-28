using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener,
OuyaSDK.IPauseListener,
OuyaSDK.IResumeListener
{

	//birds
	public Bird bird1;
	public Bird bird2;
	public Bird bird3;
	public Bird bird4;
	private Bird birdToFollow;

	public OuyaSDK.OuyaPlayer Index;
	public OuyaSDK.OuyaPlayer Index2;
	public OuyaSDK.OuyaPlayer Index3;
	public OuyaSDK.OuyaPlayer Index4;

	public AudioClip music;

	//gui
	public GUIStyle p1ScoreStyle;
	public GUIStyle p2ScoreStyle;
	public GUIStyle p3ScoreStyle;
	public GUIStyle p4ScoreStyle;
	public GUIStyle gameOverTitleStyle;
	public GUIStyle gameOverStyle;

	public Texture OUYA_A;
	public Texture OUYA_O;

	private float scoresGUIWidth = 2.0f;
	private int numberOfPlayers = 1;

	//extra gameobjects
	private GameObject ground;
	public GameObject singleGapPipeSet;
	public GameObject doubleGapPipeSet;

	//pipe generation
	private float newPipeFrequency = 2f; //every how many seconds to drop a new pipe
	private float newPipeOffSet = 35f; //how far ahead to generate new pipes
	private float newPipeMinY = -5f;	//upper bound for random new pipe height
	private float newPipeMaxY = 4f;		//lower bound for random new pipe height
	private float newPipeMinYHell = -3f;	//upper bound for random new pipe height
	private float newPipeMaxYHell = 4f;		//lower bound for random new pipe height

	private float newPipeTimer;	//initialize new pipe timer

	//follow player vars
	private Vector3 playerPos;
	private float playerXOffset = 6f;
	private bool everyBodyDead;	//variable to keep track of if every bird is dead
	private float everyBodyDeadTimer;
	private bool newHighScore = false;

	private int hellmode;
	public AudioClip hellMusic;


	// Use this for initialization
	void Start () {
		newPipeTimer = 0f;
		everyBodyDead = false;
		everyBodyDeadTimer = 0.0f;
		ground = GameObject.Find ("Ground");

		Physics2D.IgnoreLayerCollision (9, 9);	//avoid bird-bird collisions

		//get the playerpref value from player select
		numberOfPlayers = PlayerPrefs.GetInt ("numberOfPlayers");

		//create the players
			switch(numberOfPlayers){
			case 1:
				Destroy(bird2.gameObject);
				goto case 2;
			case 2:
				Destroy(bird3.gameObject);
				goto case 3;
			case 3:
				Destroy(bird4.gameObject);
				break;
			}


		//hell mode stuff
		hellmode = PlayerPrefs.GetInt ("hellmode", 0);
	
		if(hellmode == 1){
			//adjust pipe generation
			newPipeFrequency = newPipeFrequency / 3.0f;
			newPipeMaxY = newPipeMaxYHell;
			newPipeMinY = newPipeMinYHell;

			//change the song
			audio.clip = hellMusic;
			audio.loop = true;
			audio.Play();
		}

		if (PlayerPrefs.GetInt ("music",1) == 0) {
			audio.mute = true;
		}

	}

	void Awake()
	{
		OuyaSDK.registerMenuButtonUpListener(this);
		OuyaSDK.registerMenuAppearingListener(this);
		OuyaSDK.registerPauseListener(this);
		OuyaSDK.registerResumeListener(this);
		Input.ResetInputAxes();
	}
	void OnDestroy()
	{
		OuyaSDK.unregisterMenuButtonUpListener(this);
		OuyaSDK.unregisterMenuAppearingListener(this);
		OuyaSDK.unregisterPauseListener(this);
		OuyaSDK.unregisterResumeListener(this);
		Input.ResetInputAxes();
	}

	public void OuyaMenuButtonUp()
	{
	}
	
	public void OuyaMenuAppearing()
	{
	}
	
	public void OuyaOnPause()
	{
		Time.timeScale = 0;
	}
	
	public void OuyaOnResume()
	{
		Time.timeScale = 1;
	}

	// Update is called once per frame
	void Update () {

		newPipeTimer += Time.deltaTime; //add deltatime to pipe counter

		if (everyBodyDead) {
			everyBodyDeadTimer += Time.deltaTime;
		}

		//bird input switch
		switch(numberOfPlayers){
		case 4:
			if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index4)) {
				bird4.SendMessage("Jump");
			}
			goto case 3;
		case 3:
			if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index3)) {
				bird3.SendMessage("Jump");
			}
			goto case 2;
		case 2:
			if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index2)) {
				bird2.SendMessage("Jump");
			}
			goto case 1;
		case 1:
			if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index)) {
				bird1.SendMessage("Jump");
			}
			break;
		}



		//if it's time to make a new pipe make one
		if (newPipeTimer >= newPipeFrequency && !everyBodyDead) {

			float randomY = Random.Range(newPipeMinY,newPipeMaxY);
			randomY = Mathf.Round(randomY);

			Instantiate(singleGapPipeSet,
			            new Vector3(gameObject.transform.position.x + newPipeOffSet,
			            			randomY,
			            			0f),
			            Quaternion.identity);

			newPipeTimer = 0f;	//reset newPipeTimer counter
		}

		//set what bird to follow if all dead just stop moving
		float birdToFollowOffSet = 0;
		if (bird1 != null && bird1.getAlive ()) {
			birdToFollow = bird1;
		}else if(bird2 != null && bird2.getAlive ()){
			birdToFollow = bird2;
			birdToFollowOffSet = 2;
		}else if(bird3 != null && bird3.getAlive ()){
			birdToFollow = bird3;
			birdToFollowOffSet = 4;
		}else if(bird4 != null && bird4.getAlive ()){
			birdToFollow = bird4;
			birdToFollowOffSet = 6;
		}else{
			everyBodyDead = true;
		}

		//if everyones dead sit still, otherwise follow first living one
		if (!everyBodyDead) {
		//camera follow part
		playerPos = birdToFollow.transform.position;
		
		gameObject.transform.position = new Vector3 (playerPos.x + playerXOffset + birdToFollowOffSet,
		                                              gameObject.transform.position.y,
		                                              gameObject.transform.position.z);
	
		}

		if (everyBodyDead) {
			GameOver();
		}
	}

	void OnGUI(){
		//print player scores using fallthrough, case is player number
		switch(numberOfPlayers){
			case 4:
				GUI.Label(new Rect( ((Screen.width * 4.0f/((float) numberOfPlayers + 1.0f)) - 50.0f), (Screen.height / 8f), 100, 40), bird4.getScore().ToString(), p4ScoreStyle);
				goto case 3;
			case 3:
				GUI.Label(new Rect( ((Screen.width * 3.0f/((float) numberOfPlayers + 1.0f)) - 50.0f), (Screen.height / 8f), 100, 40), bird3.getScore().ToString(), p3ScoreStyle);
				goto case 2;
			case 2:
				GUI.Label(new Rect( ((Screen.width * 2.0f/((float) numberOfPlayers + 1.0f)) - 50.0f), (Screen.height / 8f), 100, 40), bird2.getScore().ToString(), p2ScoreStyle);
				goto case 1;
			case 1:
				GUI.Label(new Rect( ((Screen.width * 1.0f/((float) numberOfPlayers + 1.0f)) - 50.0f), (Screen.height / 8f), 100, 40), bird1.getScore().ToString(), p1ScoreStyle);
			break;
		}

		//highscore and restart screen
		if (everyBodyDead) {
			//Game Over Title
			GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (3.0f / 10.0f) - 20.0f), 100, 40), "Game Over", gameOverTitleStyle);

			if(numberOfPlayers > 1){
				//if there was a tie
				if(getWinningPlayerNumber() == 0.0f){
					GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (5.0f / 10.0f) - 20.0f), 100, 40), "Tie - No Winner", gameOverStyle);

				}else{
					//no tie
					//winning player and score
					GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (5.0f / 10.0f) - 20.0f), 100, 40), "Player " + getWinningPlayerNumber().ToString() + " Wins with " + getWinningPlayerScore() + " Points!", gameOverStyle);
				}
			}else{
				GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (5.0f / 10.0f) - 20.0f), 100, 40), "You got " + getWinningPlayerScore() + " Points!", gameOverStyle);
			}
			//high score
			if((getWinningPlayerScore() > PlayerPrefs.GetFloat("highScore")) || newHighScore){
				//new high score!
				newHighScore = true;	//bool to keep bringing the program through this loop after new high score is saved to playerprefs as high score

				//save high score to playerprefs
				PlayerPrefs.SetFloat("highScore",getWinningPlayerScore());
				PlayerPrefs.Save();

				//print Yay message for new high score
				GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (6.0f / 10.0f) - 20.0f), 100, 40), "New High Score! - " + PlayerPrefs.GetFloat("highScore").ToString(), gameOverStyle);
			
			}else{
				//show high score
				GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (6.0f / 10.0f) - 20.0f), 100, 40), "High Score - " + PlayerPrefs.GetFloat("highScore").ToString(), gameOverStyle);

			}

			//replay and back text
			GUI.Label(new Rect( ((Screen.width / 2.0f) - 50.0f), (Screen.height * (8.0f / 10.0f) - 20.0f), 100, 40), "    Restart        Back", gameOverStyle);

			GUI.DrawTexture(new Rect( ((Screen.width / 2.0f) - 35.0f - 250.0f), ((Screen.height * (8.0f / 10.0f)) - 50.0f), 70, 80), OUYA_O);
			GUI.DrawTexture(new Rect( ((Screen.width / 2.0f) - 35.0f + 120.0f), ((Screen.height * (8.0f / 10.0f)) - 50.0f), 70, 80), OUYA_A);

			if(everyBodyDeadTimer > 0.5f){
				//restart
				if(OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index)){
					Application.LoadLevel(Application.loadedLevel);
				}
				//back to player select
				if(OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_A, Index)){
					Application.LoadLevel("Player Select");
				}
			}
		}
	}

	//game over
	void GameOver(){
		//stop ground scrolling
		ground.BroadcastMessage ("scrollOff");
	}

	/**
	 * returns number of player with highest score up to numberOfPlayers
	 * 0 is returned if there was a tie
	 * */
	float getWinningPlayerNumber(){
		float winner = 0.0f; //var to hold winning index
		float[] scores = {0,0,0,0};

		//copy scores of active players into scores array
		switch(numberOfPlayers){
		case 4:
			scores[3] = bird4.getScore();
			goto case 3;
		case 3:
			scores[2] = bird3.getScore();
			goto case 2;
		case 2:
			scores[1] = bird2.getScore();
			goto case 1;
		case 1:
			scores[0] = bird1.getScore();
			break;
		}

		//was there a tie? aka multiple occurences of the highest score
		if (occurances (scores, maxInFloatArray (scores)) == 1.0f) {
			//no tie, just one winner
			for(int i = 0; i < scores.Length; i++){
				//if this player got the high score
				if(scores[i] == maxInFloatArray(scores)){
					winner = (float) i + 1;
					print ("player " + winner.ToString() + " won");
				}
			}
		}else{
			//yes there was a tie
			print("tie detected");
			winner = 0.0f;
		}
		return winner;
	}

	/**returns the highest player score
	 **/
	float getWinningPlayerScore(){
		float best = 0.0f; //var to hold best score
		float[] scores = {0,0,0,0};
		switch(numberOfPlayers){
		case 4:
			scores[3] = bird4.getScore();
			goto case 3;
		case 3:
			scores[2] = bird3.getScore();
			goto case 2;
		case 2:
			scores[1] = bird2.getScore();
			goto case 1;
		case 1:
			scores[0] = bird1.getScore();
			break;
		}

		//top score
		best = maxInFloatArray (scores);
		return best;
	}

	float occurances(float[] inputt, float lookingFor){
		float counter = 0;
		foreach(float current in inputt){
			if(current == lookingFor){
				counter += 1.0f;
			}
		}
		return counter;
	}

	float maxInFloatArray(float[] inputt){
		float max = 0;
		foreach(float current in inputt){
			if(current > max){
				max = current;
			}
		}
		return max;
	}

}
