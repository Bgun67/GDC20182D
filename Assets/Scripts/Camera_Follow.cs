﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
	public GameObject camPrefab;
	[HideInInspector]
	public Camera mainCamera;
	public float cameraOffset = 5f;
	float originalOffset = 5f;
	Quaternion originalCameraRotation;
	public Vector3 centerOffset;
	[HideInInspector]
	public int playerNum;
	Rigidbody rb;
	Game_Controller gameController;
	void Awake(){

		rb = GetComponent<Rigidbody>();
		gameController = FindObjectOfType<Game_Controller>();

		SetupCamera();
		originalCameraRotation = mainCamera.transform.rotation;
		originalOffset = cameraOffset;
	}
	void SetupCamera(){
		if(mainCamera == null){
			mainCamera = Instantiate(camPrefab).GetComponent<Camera>();
		}
		float _x = Mathf.Clamp01(playerNum - 1) * 0.5f;
		float _y = (playerNum) % 2 * 0.5f;
		float _width = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 2) * 0.5f;
		float _height = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 1) * 0.5f;
		mainCamera.rect = new Rect(_x, _y, _width, _height);

	}
	void Update(){
		FollowTarget();
	}
	void FollowTarget()
	{
		Vector3 _position = rb.worldCenterOfMass+centerOffset;

		//Find any Intersecting colliders
		RaycastHit _hit;
		Debug.DrawLine(_position, mainCamera.transform.position);
		if (Physics.Raycast(_position, mainCamera.transform.position-_position, out _hit, -originalOffset+0.6f)){
			cameraOffset = Mathf.Lerp(cameraOffset,Mathf.Clamp(-_hit.distance+0.1f,originalOffset,-1f),0.1f);
		}
		else
		{
			cameraOffset = Mathf.Lerp(cameraOffset,originalOffset,0.1f);
		}

		//new position for the camera
		Vector3 camPosition;

		camPosition = (_position)+ mainCamera.transform.forward*cameraOffset;


		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camPosition, 0.3f);
		mainCamera.transform.LookAt(_position);
		//PivotCam();
	}
	void PivotCam()
	{
		//mainCamera.transform.RotateAround(transform.position, Vector3.up, lookHorizontal/2f);
	}
	void ResetCam()
	{
		mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, originalCameraRotation, 0.4f);
	}
}
