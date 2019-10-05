using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
	Rigidbody rb;
	public float runMultiplier = 1f;
	float forceFactor = 7f;
	Vector3 lastVelocity;
	float lastRotation;
	bool lockedToEnemy;
	GameObject lockedEnemy;
	public ParticleSystem dust;
	Camera_Follow camController;
	Animator anim;
	void Start(){
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		camController = GetComponent<Camera_Follow>();
	}
	public void Move(float _h, float _v)
	{
		Vector3 camRight = camController.mainCamera.transform.right;
		//get previous upward velocity
		float _yVelocity = rb.velocity.y;
		//Move Player
		//Find look forward and get the vector parallel to the ground
		Vector3 _camForward = Vector3.Cross(camRight, Vector3.up);
		_camForward = new Vector3(_camForward.x, 0f, _camForward.z);
		Vector3 _currentXYVelocity = Vector3.Scale(rb.velocity, new Vector3(1f,0f,1f));
		Vector3 _XYVelocity = Vector3.ClampMagnitude(_camForward* _v*runMultiplier* forceFactor+camRight*forceFactor*_h*runMultiplier, forceFactor*runMultiplier);
		rb.velocity = Vector3.Lerp(_currentXYVelocity,_XYVelocity,0.4f);


		//Reset upwards velocity
		rb.velocity += new Vector3(0f, _yVelocity, 0f);
		Look(rb.velocity.x, rb.velocity.z);

	}
	public void Jump()
	{
        if (!anim.GetBool("Grounded"))
        {
            return;
        }
        //show animation
        anim.SetBool("Jump", true);
        anim.SetBool("Grounded", false);
	}

	void AddUpVelocity()
	{
        if (!anim.GetBool("Grounded"))
        {
            return;
        }
		//add upwards velocity to current velocity
		Vector3 _oldVelocity = rb.velocity;
        rb.velocity = new Vector3(_oldVelocity.x, 5f, _oldVelocity.z);
        anim.SetBool("Jump", false);
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
	public void AnimateMovement()
	{
		Vector3 _localVelocity = transform.InverseTransformVector(rb.velocity);
		Vector3 _localAcceleration = (_localVelocity - lastVelocity) / Time.deltaTime;
		lastVelocity = _localVelocity;
		float _angularRotation = transform.rotation.eulerAngles.y;
		float _angularVelocity = lastRotation - _angularRotation;
		lastRotation = _angularRotation;

		anim.SetFloat("Run Speed", _localVelocity.z+_angularVelocity/-4f);
		anim.SetFloat("Direction", _localVelocity.x);

		if (_localAcceleration.z > 10f)
		{
			if (dust != null && !dust.isPlaying)
			{
				dust.Play();
			}
		}
		if (_localVelocity.y < -3f)
		{
			anim.SetFloat("Fall", -_localVelocity.y);
		}
		else if (anim.GetFloat("Fall")>0)
		{
			anim.SetFloat("Fall", 0f);
		}


	}
}
