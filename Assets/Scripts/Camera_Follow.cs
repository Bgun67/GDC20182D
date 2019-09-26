using System.Collections;
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
		Vector3 _position = rb.worldCenterOfMass;

		//Find any Intersecting colliders
		RaycastHit _hit;
		Debug.DrawRay(rb.worldCenterOfMass, (mainCamera.transform.position - rb.worldCenterOfMass) * (-originalOffset + 0.6f));
		if (Physics.Raycast(rb.worldCenterOfMass, mainCamera.transform.position-rb.worldCenterOfMass, out _hit, -originalOffset+0.6f)){
			cameraOffset = Mathf.Lerp(cameraOffset,Mathf.Clamp(-_hit.distance+0.1f,originalOffset,-1f),0.1f);
		}
		else
		{
			cameraOffset = Mathf.Lerp(cameraOffset,originalOffset,0.1f);
		}

		//new position for the camera
		Vector3 camPosition;

		camPosition = (_position)+ centerOffset+ mainCamera.transform.forward*cameraOffset;


		mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, camPosition, 1f);
		mainCamera.transform.LookAt(_position+centerOffset);
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
