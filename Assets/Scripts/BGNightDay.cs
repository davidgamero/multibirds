using UnityEngine;
using System.Collections;

public class BGNightDay : MonoBehaviour {

	public Texture bgNight;
	public Texture bgHell;

	// Use this for initialization
	void Start () {
		if(PlayerPrefs.GetInt("hellmode",0) == 1){
			gameObject.guiTexture.texture = bgHell;
		}else{
			gameObject.guiTexture.texture = bgNight;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/**Make the background night**/
	void setBGNight(){
		gameObject.transform.position = new Vector3 (gameObject.transform.position.x,
		                                                1.0f,
		                                                gameObject.transform.position.z);
	}

	/**Make the background day**/
	void setBGDay(){
		gameObject.transform.position = new Vector3 (gameObject.transform.position.x,
		                                             -1.0f,
		                                             gameObject.transform.position.z);
	}

	/**Randomly choose night or day background**/
	void setBGRandom(){
		if (Random.Range(0.0f,2.0f) >= 1.0f) {
			setBGNight();
		}else{
			setBGDay();
		}
	}
}
