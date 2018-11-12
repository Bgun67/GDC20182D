using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum AttackType
{
	//use this for critical Attack in health
	None,
	//use this for a basic attack
	Default,
	Slime,
	Fire,
	Water,
	Ice,
	Grass,
	Earth
	//etc

}
[System.Serializable]
public class AttackClass
{
	public UnityEvent function;
	public float damage;
	public AttackType type = AttackType.Default;
	public bool available = true;
}
public class Player_Controller : MonoBehaviour
{
    #region Movement
    [Header("Movement")]
    float vertical;
    float horizontal;
    //Amount of force applied to player
    public float forceFactor = 5f;
    //Determines which colliders the rays should hit
    public LayerMask jumpMask;
    Rigidbody rb;
	public Animator anim;
	bool rolling;
	#endregion
	#region Camera
	[Header("Camera")]
    [Tooltip("Assigned automatrically if left empty")]
    public Camera mainCamera;
    //Placement of camera with respect to player
    public Vector3 cameraOffset;
    //originalCam rotation as a rotation type
    Quaternion originalCameraRotation;
	public float cameraSafeBoundary = 0.05f;
	#endregion
	#region Weapon
	[Header("Weapon")]
    public AttackClass[] attacks;
	public AttackClass currentAttack;
	public GameObject icePrefab;
	#endregion
	#region "UI"
	public Text infoText;
    public Text healthText;
    public Health healthScript;
    #endregion
    public Scene currentScene;
	public Scene lastScene;
	public int lastDoorNumber;
    public bool isPlayerDead;
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
		//mainCamera = this.GetComponentInChildren<Camera>();
        originalCameraRotation = mainCamera.transform.rotation;
        healthScript = this.GetComponent<Health>();
		healthScript.HealthChanged += UpdateHealth;

		SwitchAttacks(0);

        //Check to make sure there is a scene to return to
        if (lastScene.name == null)
        {
            lastScene = SceneManager.GetActiveScene();
        }
		if (currentScene.name == null)
        {
            currentScene = SceneManager.GetActiveScene();
        }
        //check if the player has fallen every 1 second
        InvokeRepeating("CheckFall", 1f, 1f);

	

	}
	
	// Update is called once per frame
	void Update () {
		//Get Input
		vertical = Input.GetAxis("Vertical "+(playerNum+1));
		horizontal = Input.GetAxis("Horizontal "+(playerNum+1));

		if (Input.GetKey("e")){
			ChooseAttack(horizontal, vertical);
		}
		//is input greater than 0
		else if(Vector2.SqrMagnitude(new Vector2(vertical, horizontal))>0f&&!rolling){
		
			//Moves the player using velocities
			Move(horizontal, vertical);
			//Turns the player to face direction of movement
			Look(horizontal, vertical);
			
		}
		if(Input.GetButtonDown("Jump "+(playerNum+1))&&CheckGrounded()){
			Jump();
		}
		Attack();
		CameraFollow();

	}
	public void SetupPlayer()
	{
		gameController = FindObjectOfType<Game_Controller>();
		//Adjust Camera to fit players on screen
		float _x = Mathf.Clamp01(playerNum - 1) * 0.5f;
		float _y = (playerNum) % 2 * 0.5f;
		float _width = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 2) * 0.5f;
		float _height = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 1) * 0.5f; 
		mainCamera.rect = new Rect(_x,_y,_width ,_height);
		ColorPlayer();
	}
	 void ColorPlayer()
	{
		this.transform.GetComponentInChildren<Renderer>().material.color = playerColors[playerNum];
	}
	#region Movement
	void Move(float _h, float _v){
		
		//get previous upward velocity
		float _yVelocity = rb.velocity.y;
		//Move Player
		rb.velocity = new Vector3(_h,0f, _v)*forceFactor;
		
		
		//Reset upwards velocity
		rb.velocity += new Vector3(0f,_yVelocity,0f);
		
	}
	void Look(float _h, float _v){
		//Point player in proper direction
		
	  //Calculate atan to find angle convert from rads Lerps the angle, result is not exactly the angle
    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.Atan(_h / _v) + Mathf.Sign(_v) * 90f - 90f, 0f), 0.2f);


		
		
	}
	void Jump(){
		//add upwards velocity to current velocity
		Vector3 _oldVelocity = rb.velocity;
		rb.velocity = Vector3.up*5f+_oldVelocity;
		//FindObjectOfType<Physics_Helper>().MoveTo(this.gameObject,new Vector3(0f,0f,0f),10f);

	}
	bool CheckGrounded(){
		bool _grounded = false;
		//draw a laser downwards and see if it hits anything
		RaycastHit _hit;
		if(Physics.Raycast(this.transform.position,Vector3.down,out _hit,0.1f,jumpMask,QueryTriggerInteraction.Ignore)){
			//we've hit something, there is something below the player
			_grounded = true;
		}
		
		return _grounded;

	}

	#endregion
	void CameraFollow()
	{
		Vector3 _position = Vector3.zero;
		int _connectedPlayers = 0;

		this.mainCamera.transform.rotation = originalCameraRotation;

		GameObject[,] _playerGrid = new GameObject[2, 2];

		//Add all players within range
		foreach (GameObject _player in gameController.players)
		{

			//find if other player is merge zone
			float _distance = Vector3.Distance(_player.transform.position, this.transform.position);
			if (_distance < 12f)
			{

				//add position 
				_position += _player.transform.position;
				Rect _camRect = _player.GetComponent<Player_Controller>().mainCamera.rect;
				_playerGrid[(int)(_camRect.x * 2f), (int)(_camRect.y * 2f)] = _player;
				_connectedPlayers++;
			}

		}
		if (_connectedPlayers > 1)
		{
			//be careful these change
			GameObject _player00 = _playerGrid[0, 0];
			GameObject _player10 = _playerGrid[1, 0];

			if (_player00 != null && _player10 != null)
			{
				//swap player cam at bottom left with bottom right if positioned incorrectly
				Camera _cam00 = _player00.GetComponent<Player_Controller>().mainCamera;
				Camera _cam10 = _player10.GetComponent<Player_Controller>().mainCamera;

				if (_player00.transform.position.x > _player10.transform.position.x)
				{
					_playerGrid[0, 0].GetComponent<Player_Controller>().mainCamera = _cam10;
					_playerGrid[1, 0].GetComponent<Player_Controller>().mainCamera = _cam00;

					_playerGrid[0, 0] = _player10;
					_playerGrid[1, 0] = _player00;
				}
			}
			//be careful these change
			_player00 = _playerGrid[0, 0];
			GameObject _player01 = _playerGrid[0, 1];

			if (_player00 != null && _player01 != null)
			{
				//swap player cam at bottom left with top left if positioned incorrectly


				if (_player00.transform.position.z > _player01.transform.position.z)
				{
					Camera _cam00 = _player00.GetComponent<Player_Controller>().mainCamera;
					Camera _cam01 = _player01.GetComponent<Player_Controller>().mainCamera;


					_playerGrid[0, 0].GetComponent<Player_Controller>().mainCamera = _cam01;
					_playerGrid[0, 1].GetComponent<Player_Controller>().mainCamera = _cam00;

					_playerGrid[0, 0] = _player01;
					_playerGrid[0, 1] = _player00;
				}
			}
			//be careful these change
			_player01 = _playerGrid[0, 1];
			GameObject _player11 = _playerGrid[1, 1];

			if (_player01 != null && _player11 != null)
			{
				//swap player cam at top left with top right if positioned incorrectly

				Camera _cam01 = _player01.GetComponent<Player_Controller>().mainCamera;
				Camera _cam11 = _player11.GetComponent<Player_Controller>().mainCamera;
				if (_player01.transform.position.x > _player11.transform.position.x)
				{
					_playerGrid[0, 1].GetComponent<Player_Controller>().mainCamera = _cam11;
					_playerGrid[1, 1].GetComponent<Player_Controller>().mainCamera = _cam01;

					_playerGrid[0, 1] = _player11;
					_playerGrid[1, 1] = _player01;
				}
			}
			//be careful these change
			_player11 = _playerGrid[1, 1];
			_player10 = _playerGrid[1, 0];

			if (_player11 != null && _player10 != null)
			{
				//swap player cam at top right with bottom right if positioned incorrectly
				Camera _cam11 = _player11.GetComponent<Player_Controller>().mainCamera;
				Camera _cam10 = _player10.GetComponent<Player_Controller>().mainCamera;
				if (_player10.transform.position.z > _player11.transform.position.z)
				{
					_playerGrid[1, 0].GetComponent<Player_Controller>().mainCamera = _cam11;
					_playerGrid[1, 1].GetComponent<Player_Controller>().mainCamera = _cam10;

					_playerGrid[1, 0] = _player11;
					_playerGrid[1, 1] = _player10;
				}
			}
		}


		//average positions only if there are 2 or 4 players connected together
		Vector3 camPosition;

		camPosition = (_position / _connectedPlayers) + cameraOffset;


		//Move camera forward or backward to create a mixed view
		if (_connectedPlayers > 1)
		{
			float _height = mainCamera.orthographicSize * 2;
			float _width = _height * Screen.width / Screen.height;
			if (_connectedPlayers > 2)
			{
				camPosition += (1 - mainCamera.rect.x * 4f) * 0.5f * _width * Vector3.left;
			}

			camPosition += (mainCamera.rect.y * 4f - 1) * 0.8f * _height * Vector3.forward;

		}
		//Move the camera slow if entering or exiting merge zone
		
		if (Vector3.Distance(camPosition, mainCamera.transform.position) < 11f)
		{
			mainCamera.transform.position = camPosition;
		}
		else
		{
						mainCamera.transform.position = camPosition;

			//move camera to positon
			//mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, camPosition, 0.1f);
		}


	}


	void Attack(){
		if (!rolling)
		{
			if (Input.GetButtonDown("Primary Attack "+(playerNum+1)))
			{
				currentWeapon.Attack(0);
			}
			if (Input.GetButtonDown("Secondary Attack "+(playerNum+1)))
			{
				currentWeapon.Attack(1);
			}
			if (Input.GetButtonDown("Roll Right "+(playerNum+1)))
			{
				RollRight();
			}
			if (Input.GetButtonDown("Roll Left "+(playerNum+1)))
			{
				RollLeft();
			}
		}
	}
    void CheckFall()
    {

        if (transform.position.y < -10f)
        {

            healthScript.TakeDamage(1000f);
        }
    }
	public void Die()
	{
		//check if player has already died

		infoText.text = "You Died";
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
		//reset health to full
		healthScript.Reset();


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
		GameObject newHit = Instantiate(hitIndicator, transform.position + Vector3.up, Quaternion.identity);
		newHit.GetComponent<HitIndicator>().SetHealth(amount);
    }
	public void RollRight()
	{
		if (CheckGrounded()&&anim.GetInteger("Attack Number")==0)
		{
			anim.SetTrigger("Roll Right");

			rb.velocity = transform.right * 10f;
			rolling = true;

		}

	}
	public void RollLeft()
	{
		if (CheckGrounded()&&anim.GetInteger("Attack Number")==0){
			anim.SetTrigger("Roll Left");

			rb.velocity = transform.right * -10f;
			rolling = true;
		}

	}
	public void ResetRoll()
	{
		rolling = false;
	}
	public void FlipSmash()
	{
		StartCoroutine(RunFlipSmash());
	}
	public IEnumerator RunFlipSmash()
	{
		//Make sure player is in the air and not attacking
		if (CheckGrounded()||anim.GetInteger("Attack Number")!=0)
		{
			yield break;
		}
		//do a flip
		anim.SetInteger("Attack Number", 1);
		//add a force upwards to keep player in air longer
		rb.velocity = Vector3.up*7f;
		//Wait a second
		yield return new WaitForSeconds(1f);
		//send the player crashing down
		rb.velocity = Vector3.down*10f;
		yield return new WaitUntil(() => CheckGrounded());
		//reset player's animation
		anim.SetInteger("Attack Number", 0);
		//check for object below
		RaycastHit _hit;
		if (!Physics.SphereCast(transform.position,0.01f, -transform.up, out _hit, 2f, jumpMask, QueryTriggerInteraction.Ignore))
		{
			yield break;
		}
		//if object below has a health component, give damage
		if (_hit.transform.GetComponent<Health>() != null)
		{
			_hit.transform.GetComponent<Health>().TakeDamage(100f);
		}
		

	}
	public void ShoulderRam()
	{
		//Make sure player is grounded and not attacking
		if (!CheckGrounded()||anim.GetInteger("Attack Number")!=0)
		{
			return;
		}
		//Ram
		anim.SetInteger("Attack Number", 2);
		
		//Damage is called by animation event
	}
	public void ForwardStrike()
	{
		RaycastHit _hit;
		//reset player's animation
		anim.SetInteger("Attack Number", 0);
		//add force forwards
		rb.velocity = transform.forward*3f;
		//check for object
		if (!Physics.SphereCast(transform.position,0.3f, transform.forward, out _hit, 2f, jumpMask, QueryTriggerInteraction.Ignore))
		{
			return;
		}
		//if object below has a health component, give damage
		if (_hit.transform.GetComponent<Health>() != null)
		{
			_hit.transform.GetComponent<Health>().TakeDamage(currentWeapon.GetStrikeDamage());
		}
	
	}

}
