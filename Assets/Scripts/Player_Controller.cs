using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_Controller : MonoBehaviour {
	#region Movement
	[Header("Movement")]
	float vertical;
	float horizontal;
	//Amount of force applied to player
	public float forceFactor = 5f;
	//Determines which colliders the rays should hit
	public LayerMask jumpMask;
	Rigidbody rb;
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
    #endregion
   #region "UI"
   public Text infoText;
    public Text healthText;
    public Health healthScript;
    #endregion
    public Scene lastScene;
    public int lastDoorNumber;
    bool isDead;

	public GameObject hitIndicator;//Hit indicator prefab

    // Use this for initialization
    void Start()
    {
        //Find the rigidbody
        rb = GetComponent<Rigidbody>();
        mainCamera = transform.GetComponentInChildren<Camera>();
        originalCameraRotation = mainCamera.transform.rotation;
        healthScript = this.GetComponent<Health>();
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

		//is input greater than 0
		if(Vector2.SqrMagnitude(new Vector2(vertical, horizontal))>0f/* &&CheckGrounded()*/){
		
			//Moves the player using velocities
			Move(horizontal, vertical);
			//Turns the player to face direction of movement
			Look(horizontal, vertical);
			
		}
		if(Input.GetKeyDown("space")&&CheckGrounded()){
			Jump();
		}
		if(Input.GetKeyDown("f")){
			Attack();
		}
		CameraFollow();

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
		
		//Calculate atan to find angle convert from rads
		transform.rotation = Quaternion.Euler(0f,Mathf.Rad2Deg*Mathf.Atan(_h/_v)+Mathf.Sign(_v)*90f-90f,0f);
		
		
	}
	void Jump(){
		//add upwards velocity to current velocity
		Vector3 _oldVelocity = rb.velocity;
		rb.velocity = Vector3.up*5f+_oldVelocity;
	}
	bool CheckGrounded(){
		bool _grounded = false;
		//draw a laser downwards and see if it hits anything
		RaycastHit _hit;
		if(Physics.Raycast(this.transform.position,Vector3.down,out _hit,0.55f,jumpMask,QueryTriggerInteraction.Ignore)){
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
		//draw a laser forward and see if it hits anything
		RaycastHit _hit;
		if(Physics.Raycast(this.transform.position,transform.forward,out _hit,2f,jumpMask,QueryTriggerInteraction.Ignore)){
			Health healthScript = _hit.transform.root.GetComponent<Health>();
			if(healthScript!=null){
				healthScript.TakeDamage(weaponDamageFactor);
			}
		}
	}
    void CheckFall()
    {
		
        if (transform.position.y<-10f)
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

    public void UpdateHealth()
    {
		//check to make sure health is assigned
        if (healthScript == null)
        {
            healthScript = this.GetComponent<Health>();
        }
		//Clear shields and get number of shields needed
        int _shieldNumber = (int)healthScript.currentHealth/10;
        healthText.text = "";
		//type shields
        for (int i = 0; i < _shieldNumber; i++)
        {
            healthText.text += " 0";
        }
		Debug.Log("Spawn hit ind");
		GameObject newHit = Instantiate(hitIndicator, transform.position + Vector3.up, Quaternion.identity);
		newHit.GetComponent<HitIndicator>().SetHealth(healthScript.lastUpdate);
    }


}
