using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {
	float vertical;
	float horizontal;
	[Tooltip("Assigned automatrically if left empty")]
	public Camera mainCamera;
	//Placement of camera with respect to player
	public Vector3 cameraOffset;
	//Amount of force applied to player
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
		rb.velocity = new Vector3(_h,0f, _v)*forceFactor;
		
		
		//Reset upwards velocity
		rb.velocity += new Vector3(0f,_yVelocity,0f);
		
	}
	void CameraFollow(){
		//Track players position but reset the rotation
		mainCamera.transform.position = this.transform.position + cameraOffset;
		mainCamera.transform.rotation = originalCameraRotation;
	}
	void Look(float _h, float _v){
		//Point player in proper direction
		
		//Calculate atan to find angle convert from rads
		transform.rotation = Quaternion.Euler(0f,Mathf.Rad2Deg*Mathf.Atan(_h/_v)+Mathf.Sign(_v)*90f-90f,0f);
		
		
	}
	
	
}
