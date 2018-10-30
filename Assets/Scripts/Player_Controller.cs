using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    #endregion
    #region Weapon
    [Header("Weapon")]
    public float weaponDamageFactor = 5f;
    public float weaponRange = 2f;
	public Weapon primaryWeapon;
	public Weapon secondaryWeapon;
	public Weapon currentWeapon;
	public Transform finger;
	#endregion
	#region "UI"
	public Text infoText;
    public Text healthText;
    public Health healthScript;
    #endregion
    public Scene currentScene;
	public Scene lastScene;
	public int lastDoorNumber;
    public bool isDead;
	Game_Controller gameController;
	#region Player Data
	public int playerNum;
	#endregion
	


    // Use this for initialization
    void Start()
    {
        //Find the rigidbody
        rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		//mainCamera = this.GetComponentInChildren<Camera>();
        originalCameraRotation = mainCamera.transform.rotation;
        healthScript = this.GetComponent<Health>();
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

		primaryWeapon = WeaponLoader.LoadWeapon(finger, 1, playerNum);
		secondaryWeapon = WeaponLoader.LoadWeapon(finger, 2, playerNum);
		secondaryWeapon.gameObject.SetActive(false);
		currentWeapon = primaryWeapon;

	}
	
	// Update is called once per frame
	void Update () {
		//Get Input
		vertical = Input.GetAxis("Vertical "+(playerNum+1));
		horizontal = Input.GetAxis("Horizontal "+(playerNum+1));

		//is input greater than 0
		if(Vector2.SqrMagnitude(new Vector2(vertical, horizontal))>0f&&!rolling){
		
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
		//multiplication added at end to fit player 3 to full screen
		float _height = 1 - Mathf.Clamp01(gameController.numberOfPlayers - 1) * 0.5f; //* Mathf.Clamp01(Mathf.Abs(gameController.numberOfPlayers - (playerNum + 1)));
		mainCamera.rect = new Rect(_x,_y,_width ,_height);
		ColorPlayer();
	}
	 void ColorPlayer()
	{

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
	void CameraFollow(){
		//Track players position but reset the rotation
		mainCamera.transform.position = this.transform.position + cameraOffset;
		mainCamera.transform.rotation = originalCameraRotation;
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
			if (Input.GetKeyDown("d"))
			{
				RollRight();
			}
			if (Input.GetKeyDown("a"))
			{
				RollLeft();
			}
		}
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
        //check if player has already died
        if (!isDead)
        {
            //reset health to full
            healthScript.Reset();
            infoText.text = "You Died";
            isDead = true;
			//Go to level spawn
			gameController.GetLevelController(currentScene.name).SpawnPlayer(this.gameObject);


		}

    }

    public void UpdateHealth()
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
		if (!Physics.Raycast(transform.position, Vector3.down, out _hit, 2f, jumpMask, QueryTriggerInteraction.Ignore))
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
		if (!Physics.Raycast(transform.position, transform.forward, out _hit, 2f, jumpMask, QueryTriggerInteraction.Ignore))
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
