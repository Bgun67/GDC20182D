using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
	Rigidbody rb;
	public float runMultiplier = 1f;
	float forceFactor = 7f;
	bool lockedToEnemy;
	GameObject lockedEnemy;
	void Start(){
		rb = GetComponent<Rigidbody>();
	}
	public void Move(float _h, float _v, Vector3 camRight)
	{

		//get previous upward velocity
		float _yVelocity = rb.velocity.y;
		//Move Player
		//Find look forward and get the vector parallel to the ground
		Vector3 _camForward = Vector3.Cross(camRight, Vector3.up);
		_camForward = new Vector3(_camForward.x, 0f, _camForward.z);
		Vector3 _currentXYVelocity = Vector3.Scale(rb.velocity, new Vector3(1f,0f,1f));
		Vector3 _XYVelocity = Vector3.ClampMagnitude(_camForward* _v*runMultiplier* forceFactor+camRight*forceFactor*_h*runMultiplier, forceFactor*runMultiplier);
		rb.velocity = Vector3.Lerp(_currentXYVelocity,_XYVelocity,0.1f);


		//Reset upwards velocity
		rb.velocity += new Vector3(0f, _yVelocity, 0f);
		Look(rb.velocity.x, rb.velocity.z);

	}
	void Look(float _h, float _v)
	{
		//Point player in proper direction

		//Calculate atan to find angle convert from rads Lerps the angle, result is not exactly the angle
		if (Mathf.Abs(_v) > 0||Mathf.Abs(_h)>0)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.Atan(_h / _v) + Mathf.Sign(_v) * 90f - 90f, 0f), 0.2f);			
		}
		if(lockedToEnemy){
			TrackEnemy();
		}
			
	}
	public void LockOnEnemy(){
		if (lockedToEnemy)
		{
			lockedToEnemy = false;
			lockedEnemy = null;
			//ResetCam();

		}
		else if (Utility.FindNearestEnemy(transform.position, out lockedEnemy))
		{
			//mainCamera.transform.LookAt(lockedEnemy.transform);
			lockedToEnemy = true;
		}

	}
	void TrackEnemy(){
		if (lockedEnemy != null)
		{
			transform.LookAt(lockedEnemy.transform, Vector3.up);
		}
		else{
			lockedEnemy = null;
			lockedToEnemy = false;
		}
	}
}
