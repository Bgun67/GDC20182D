using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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
    [Tooltip("Assigned automatically if left empty")]
    public Camera mainCamera;
    //Placement of camera with respect to player
    public Vector3 cameraOffset;
    //originalCam rotation as a rotation type
    Quaternion originalCameraRotation;
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
		mainCamera = transform.GetComponentInChildren<Camera>();
        originalCameraRotation = mainCamera.transform.rotation;
        healthScript = this.GetComponent<Health>();
		healthScript.HealthChanged += UpdateHealth;

		SwitchAttacks(0);

		//Check to make sure there is a scene to return to
		if (lastScene.name == null)
        {
            lastScene = SceneManager.GetActiveScene();
        }
        //check if the player has fallen every 1 second
        InvokeRepeating("CheckFall", 1f, 1f);

	
	}
    // Update is called once per frame
    void Update () {
		//Get Input
		vertical = Input.GetAxis("Vertical");
		horizontal = Input.GetAxis("Horizontal");

		if (Input.GetButton("Change Attack")){
			ChooseAttack(horizontal, vertical);
		}
		//is input greater than 0
		else if(Vector2.SqrMagnitude(new Vector2(vertical, horizontal))>0f&&!rolling){
		
			//Moves the player using velocities
			Move(horizontal, vertical);
			//Turns the player to face direction of movement
			Look(horizontal, vertical);
			
		}
		if(Input.GetKeyDown("space")&&CheckGrounded()){
			Jump();
		}
		Attack();
		CameraFollow();
		

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
		else 	//on the left side of the clock
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
			AttackClass _attack = attacks[_attackNum];
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
			if (Input.GetButtonDown("Fire1"))
			{
				currentAttack.function.Invoke();
			}
			if (Input.GetKey("s")){
				FlipSmash();
			}

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
	#endregion
	#region Movement
	void Move(float _h, float _v)
	{

		//get previous upward velocity
		float _yVelocity = rb.velocity.y;
		//Move Player
		rb.velocity = new Vector3(_h, 0f, _v) * forceFactor;


		//Reset upwards velocity
		rb.velocity += new Vector3(0f, _yVelocity, 0f);
	

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
            //go through last door
            foreach (Door _door in FindObjectsOfType<Door>())
            {
                if (_door.doorNumber == lastDoorNumber)
                {
                    _door.OpenDoor();
                    break;
                }
            }
            isDead = false;

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
	#region Attacks
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
			_hit.transform.GetComponent<Health>().TakeDamage(currentAttack.damage, currentAttack.type);
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
			_hit.transform.GetComponent<Health>().TakeDamage(currentAttack.damage, currentAttack.type);
		}
	
	}
	public void Ice()
	{
		RaycastHit _hit;
		if (!Physics.SphereCast(transform.position+rb.centerOfMass,0.3f, transform.forward, out _hit, 2f, jumpMask, QueryTriggerInteraction.Ignore))
		{
			return;
		}
		Health _hitHealth = _hit.transform.GetComponent<Health>();
		if (_hitHealth != null)
		{
			Destroy(Instantiate(icePrefab, _hitHealth.transform.position, Quaternion.identity, _hitHealth.transform),5f);
			
		}
	}
	
	#endregion

}
