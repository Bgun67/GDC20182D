using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {
	float vertical;
	float horizontal;
	[Tooltip("Assigned automatrically if left empty")]
	public Camera mainCamera;
	public Vector3 cameraOffset;
	//Amou nt of force applied to player
	public float forceFactor = 5f;
	Rigidbody rb;
	Quaternion originalCameraRotation;
	// Use this for initialization
	void Start () {
		//Find the rigidbody
		rb = GetComponent<Rigidbody>();
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		originalCameraRotation = mainCamera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		//Get Input
		vertical = Input.GetAxis("Vertical");
		horizontal = Input.GetAxis("Horizontal");

		//is input greater than 0
		if(Vector2.SqrMagnitude(new Vector2(vertical, horizontal))>0f){
		
			//Moves the player using velocities
			Move(horizontal, vertical);
			//Turns the player to face direction of movement
			Look(horizontal, vertical);
			
		}
		CameraFollow();

	}
	void Move(float _h, float _v){
		
		//get previous upward velocity
		float _yVelocity = rb.velocity.y;
		//Move Player
		if(Mathf.Abs(_h)>Mathf.Abs(_v)){
			rb.velocity = new Vector3(_h,0f, 0f)*forceFactor;
		}
		else{
			rb.velocity = new Vector3(0f,0f, _v)*forceFactor;
		
		}
		//Reset upwards velocity
		rb.velocity += new Vector3(0f,_yVelocity,0f);
		
	}
	void CameraFollow(){
		mainCamera.transform.position = this.transform.position + cameraOffset;
		mainCamera.transform.rotation = originalCameraRotation;
	}
	void Look(float _h, float _v){
		//Point player in proper direction
		
		//Is the vertical movement greater than horizontal
		if(Mathf.Abs(_v)>Mathf.Abs(_h)){
			//align to angle 0 or 180 based on sign of vertical
			transform.rotation = Quaternion.Euler(0f,Mathf.Sign(_v)*90f-90f,0f);

		}
		else{
			//Align to 90 or -90 based on sign of horizontal
			transform.rotation = Quaternion.Euler(0f,Mathf.Sign(_h)*90f,0f);



		}
	}
	
	
}
