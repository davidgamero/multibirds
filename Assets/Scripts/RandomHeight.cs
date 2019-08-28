using UnityEngine;
using System.Collections;

public class RandomHeight : MonoBehaviour {

	private float newPipeMinY = -5f;
	private float newPipeMaxY = 5f;
	private float newPipeMinYHell = -3f;	//upper bound for random new pipe height in hellmode
	private float newPipeMaxYHell = 4f;		//lower bound for random new pipe height in hellmode

	private int hellmode;

	// Use this for initialization
	void Start () {

		hellmode = PlayerPrefs.GetInt ("hellmode", 0);
		if(hellmode == 1){
			newPipeMaxY = newPipeMaxYHell;
			newPipeMinY = newPipeMinYHell;
		}


		//move to a random y within min and max
		float randomY = Random.Range(newPipeMinY,newPipeMaxY);
		randomY = Mathf.Round(randomY);
		gameObject.transform.position = (new Vector3 (gameObject.transform.position.x,
		                                              randomY,
		                                              gameObject.transform.position.z));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
