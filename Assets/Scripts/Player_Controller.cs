using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[System.Serializable]
[System.Flags]
public enum AttackType
{
	// Use this for critical and ineffective attacks 
    // AttackType can be a mix of types
    // Change To Long If Over 16 Attack Types are required

    None    = 0,
    Normal  = 1 << 0,
	Melee   = 1 << 1,
    Fire    = 1 << 2,
    Water   = 1 << 3,
    Ice     = 1 << 4,
    Grass   = 1 << 5,
    Earth   = 1 << 6,
    Slime   = 1 << 7,
	
}



public class Player_Controller : MonoBehaviour
{

	#region Movement
	[Header("Movement")]
	Player_Movement movement;
	float moveVertical;
	float previousMoveVertical;
	float moveHorizontal;
	float lookVertical;
	float lookHorizontal;
	float lastRotation;
	//multiplier for running lock on speeds
	Vector3 lastVelocity;
	//Amount of force applied to player
	public float forceFactor = 5f;
	//Determines which colliders the rays should hit
	public LayerMask jumpMask;
	public ParticleSystem dust;
	Rigidbody rb;
	public Animator anim;
	int lastTapFrame;
	bool rolling;
	#endregion
	#region Camera
	[Header("Camera")]
	[Tooltip("Assigned automatically if left empty")]
	public Camera mainCamera;
	//Placement of camera with respect to player
	float cameraOffset;
	public float originalOffset;
	public Vector3 centerOffset;
	bool lockedToEnemy;
	GameObject lockedEnemy;
	//originalCam rotation as a rotation type
	Quaternion originalCameraRotation;
	#endregion
	#region Weapon
	[Header("Weapon")]
	public Attack[] attacks;
	public Attack currentAttack;
	#endregion
	#region "UI"
	public Text infoText;
	public Text healthText;
	public Health healthScript;
	#endregion
	public Scene currentScene;
	public Scene lastScene;
	public int lastDoorNumber;
	bool isDead;
	Game_Controller gameController;
	public Color[] playerColors;
	#region Player Data
	public int playerNum;
	#endregion


	public GameObject hitIndicator;//Hit indicator prefab

	// Use this for initialization
	void Start()
	{
		//Find the rigidbody
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		movement = GetComponent<Player_Movement>();
		originalCameraRotation = mainCamera.transform.rotation;		
		healthScript = this.GetComponent<Health>();
		healthScript.HealthChanged += UpdateHealth;


		SwitchAttacks(0);

		//Check to make sure there is a scene to return to
		if (lastScene.name == null)
		{
			lastScene = SceneManager.GetActiveScene();
			currentScene = SceneManager.GetActiveScene();
		}
		//check if the player has fallen every 1 second
		InvokeRepeating("CheckFall", 1f, 1f);


	}
	// Update is called once per frame
	void Update()
	{
		//Get Input
		moveVertical = Input_Manager.GetAxis("Move Vertical " + (playerNum + 1));
		moveHorizontal = Input_Manager.GetAxis("Move Horizontal " + (playerNum + 1));
		lookVertical = Input_Manager.GetAxis("Look Vertical " + (playerNum + 1));
		lookHorizontal = Input_Manager.GetAxis("Look Horizontal " + (playerNum + 1));
		if (Input_Manager.GetAxisRaw("Move Vertical " + (playerNum + 1))>0 &&previousMoveVertical==0f)
		{
			if (Time.frameCount-lastTapFrame < 20)
			{
				RollForward();
			}
			else
			{
				lastTapFrame = Time.frameCount;
			}
		}
		previousMoveVertical = Input_Manager.GetAxisRaw("Move Vertical " + (playerNum + 1));
		if (Input.GetButton("Change Weapon " + (playerNum + 1)))
		{
			ChooseAttack(moveHorizontal, moveVertical);
		}
		if (Input.GetKey(KeyCode.LeftShift)){
			movement.runMultiplier = 1.75f;
		}
		else
		{
			movement.runMultiplier = 1f;
		}
		if (Input.GetKeyDown("k"))
		{
			movement.LockOnEnemy();
		}

		//is input greater than 0
		else if (!rolling)
		{
			if (CheckGrounded()&&Vector2.SqrMagnitude(new Vector2(moveHorizontal, moveVertical)) > 0.1f)
			{
				//Moves the player using velocities
				movement.Move(moveHorizontal, moveVertical, mainCamera.transform.right);
			}
		}
		if (Input.GetButtonDown("Jump " + (playerNum + 1)) && CheckGrounded())
		{
			movement.Jump();
		}
		Attack();
		CameraFollow();
		movement.AnimateMovement();


	}
	public void SetupPlayer()
	{
		gameController = FindObjectOfType<Game_Controller>();
		//Adjust Camera to fit players on screen
		float _x = Mathf.Clamp01(playerNum - 1) * 0.5f;
		float _y = (playerNum) % 2 * 0.5f;
		float _width = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 2) * 0.5f;
		float _height = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 1) * 0.5f;
		mainCamera.rect = new Rect(_x, _y, _width, _height);
		ColorPlayer();
	}
	void ColorPlayer()
	{
		this.transform.GetComponentInChildren<Renderer>().material.color = playerColors[playerNum];
	}
	#region Attacks

	void ChooseAttack(float _h, float _v)
	{
		//     1
		//  8     2
		// 7   0   3
		//  6     4
		//     5
		//on the right side of the clock
		if (_h > 0.5f)
		{
			//upper right
			if (_v > 0.5f)
			{
				// attempt to switch to attack 2 if available
				SwitchAttacks(2);
			}
			//bottom right
			else if (_v < -0.5f)
			{
				// attempt to switch to attack 2 if available
				SwitchAttacks(4);
			}
			//middle right
			else
			{
				SwitchAttacks(3);
			}
		}
		else    //on the left side of the clock
		if (_h < -0.5f)
		{
			//upper left
			if (_v > 0.5f)
			{
				// attempt to switch if available
				SwitchAttacks(8);
			}
			//bottom right
			else if (_v < -0.5f)
			{
				// attempt to switch to attack 2 if available
				SwitchAttacks(6);
			}
			//middle right
			else
			{
				SwitchAttacks(7);
			}
		}
		//center
		else
		{
			//upper center
			if (_v > 0.5f)
			{
				SwitchAttacks(1);
			}
			//bottom 
			else if (_v < -0.5f)
			{
				SwitchAttacks(5);
			}
			//middle center
			else
			{
				SwitchAttacks(0);
			}
		}
	}
	void SwitchAttacks(int _attackNum)
	{
		if (_attackNum >= 0 && _attackNum < attacks.Length)
		{
			Attack _attack = attacks[_attackNum];
			if (_attack.available)
			{
				currentAttack = _attack;
			}
		}

	}
	void Attack()
	{
		if (!rolling)
		{
			if (Input.GetButtonDown("Primary Attack " + (playerNum + 1)))
			{
				currentAttack.RunAnimation();
			}
			if (Input.GetButtonDown("Secondary Attack " + (playerNum + 1)))
			{
				FlipSmash();
			}

		}
		if (Input.GetButtonDown("Roll " + (playerNum + 1)))
		{
			RollForward();
		}
		
	}
	#endregion

	bool CheckGrounded()
	{
		bool _grounded = false;
		//draw a laser downwards and see if it hits anything
		RaycastHit _hit;
		if (Physics.SphereCast(this.transform.position+rb.centerOfMass,0.2f, Vector3.down, out _hit, 1.1f, jumpMask, QueryTriggerInteraction.Ignore))
		{
			//we've hit something, there is something below the player
			_grounded = true;
		}

		return _grounded;

	}

	void CameraFollow()
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
		PivotCam();
	}
	void PivotCam()
	{
		mainCamera.transform.RotateAround(transform.position, Vector3.up, lookHorizontal/2f);
	}
	void ResetCam()
	{
		mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, originalCameraRotation, 0.4f);
	}


	void CheckFall()
	{

		if (transform.position.y < -10f)
		{

			Die();
		}
	}
	public void Die()
	{

		//Go to level spawn
		Level_Controller _levelController = gameController.GetLevelController(currentScene.name);
		if (_levelController != null)
		{
			_levelController.SpawnPlayer(this.gameObject);
		}
		else
		{
			print("No Level Controller in this scene please add one");
		}
	}

	public void UpdateHealth(float amount)
	{
		//check to make sure health is assigned
		if (healthScript == null)
		{
			healthScript = this.GetComponent<Health>();
		}
		//Clear shields and get number of shields needed
		int _shieldNumber = (int)healthScript.currentHealth / 10;
		healthText.text = "";
		//type shields
		for (int i = 0; i < _shieldNumber; i++)
		{
			healthText.text += " 0";
		}
		if (hitIndicator != null)
		{
			GameObject newHit = Instantiate(hitIndicator, transform.position + Vector3.up, Quaternion.identity);
			newHit.GetComponent<HitIndicator>().SetHealth(amount);
		}
	}
	public void RollForward()
	{
		if (CheckGrounded() && anim.GetInteger("Attack Number") == 0)
		{
			anim.SetTrigger("Roll Forward");
			rb.velocity = transform.forward * 7f;
			rolling = true;

		}

	}
	public void ResetRoll()
	{
		rolling = false;
	}
	#region Attacks
	public void FlipSmash()
	{
		StartCoroutine(RunFlipSmash());
	}
	public IEnumerator RunFlipSmash()
	{
		//Make sure player is in the air and not attacking
		if (CheckGrounded() || anim.GetInteger("Attack Number") != 0)
		{
			yield break;
		}
		//do a flip
		anim.SetInteger("Attack Number", 1);
		//add a force upwards to keep player in air longer
		rb.velocity = Vector3.up * 7f;
		//Wait a second
		yield return new WaitForSeconds(1f);
		//send the player crashing down
		rb.velocity = Vector3.down * 10f;
		yield return new WaitUntil(() => CheckGrounded());
		//reset player's animation
		anim.SetInteger("Attack Number", 0);
		//check for object below
		RaycastHit _hit;
		if (!Physics.SphereCast(transform.position+rb.centerOfMass,0.2f, Vector3.down, out _hit, 1f, jumpMask, QueryTriggerInteraction.Ignore))
		{
			yield break;
		}

	}
	public void ShoulderRam()
	{
		//Make sure player is grounded and not attacking
		if (!CheckGrounded() || anim.GetInteger("Attack Number") != 0)
		{
			return;
		}
		//Ram
		anim.SetInteger("Attack Number", 2);
		//Damage is called by animation event
	}
	
	/* public void Ice()
	{
		currentAttack.effect.GetComponent<ParticleSystem>().Play();
		RaycastHit _hit;
		if (!Physics.SphereCast(transform.position + rb.centerOfMass, 0.3f, transform.forward, out _hit, 10f, jumpMask, QueryTriggerInteraction.Ignore))
		{
			return;
		}
		Health _hitHealth = _hit.transform.GetComponent<Health>();
		if (_hitHealth != null)
		{
			Destroy(Instantiate(icePrefab, _hitHealth.transform.position, Quaternion.identity, _hitHealth.transform), 5f);

		}
	}
	public void EarthQuake()
	{
		if (currentAttack.currentUses > currentAttack.maxUses)
		{
			if (Time.time > currentAttack.rechargeWait)
			{
				currentAttack.currentUses = 0;
			}
			else
			{
				return;
			}
		}
		

		currentAttack.currentUses++;
	}*/

	#endregion

}
