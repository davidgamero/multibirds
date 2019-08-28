using UnityEngine;
using System.Collections;

public class FollowX : MonoBehaviour {

	public GameObject Target;
	public float xOffset;
	private Vector3 targetPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = new Vector3 (Target.transform.position.x + xOffset,
		                                            gameObject.transform.position.y,
		                                            gameObject.transform.position.z);
	}
}
