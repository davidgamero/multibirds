using UnityEngine;
using System.Collections;

public class BirdAnimationControl : MonoBehaviour {
	
	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FreefallState(bool state){
		//send the freefalling state to the animator
		anim.SetBool("Freefalling",state);
	}

	public void HitGround(){
		//stop flapping on hitting the ground
		anim.speed = 0;
	}
}
