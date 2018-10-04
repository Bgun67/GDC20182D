using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {
	float vertical;
	float horizontal;
	//Amou nt of force applied to player
	public float forceFactor = 5f;
	Rigidbody rb;
	// Use this for initialization
	void Start () {
		//Find the rigidbody
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		//Get Input
		vertical = Input.GetAxis("Vertical");
		horizontal = Input.GetAxis("Horizontal");

		//Moves the player using velocities
		Move(horizontal, vertical);
		//Turns the player to face direction of movement
		Look(horizontal, vertical);

	}
	void Move(float _h, float _v){
		//Move Player
		rb.velocity = new Vector3(_h,0f, _v)*forceFactor;
	}
	void Look(float _h, float _v){
		//Point player in proper direction
		
		//No movement - Keep Rotation
		if(Vector2.SqrMagnitude(new Vector2(_h, _v))==0f){
			//do nothing
		}
		//Is the vertical movement greater than horizontal
		else if(Mathf.Abs(_v)>Mathf.Abs(_h)){
			//align to angle 0 or 180 based on sign of vertical
			transform.rotation = Quaternion.Euler(0f,Mathf.Sign(_v)*90f-90f,0f);

		}
		else{
			//Align to 90 or -90 based on sign of horizontal
			transform.rotation = Quaternion.Euler(0f,Mathf.Sign(_h)*90f,0f);



		}
	}
	
	
}
