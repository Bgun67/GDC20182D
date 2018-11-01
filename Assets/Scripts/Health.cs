using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
	[Tooltip("The original health of the object")]
	public float originalHealth;
	public float currentHealth;
	[Tooltip("If left empty, object is destroyed")]
	public UnityEvent dieFunction;
    public bool showHealth;
    public UnityEvent healthUpdateFunction;

	#region regen
	[Tooltip("Does the object regenerate health")]
    public bool regen;
	[Tooltip("How long after being attacked does the object start to regen health")]
    public float regenDelay = 5f;
    [Tooltip("Pause between health increments")]
    public float regenTime = 1f;
    [Tooltip("The amount of health each increment increases current health")]
    public float regenAmount = 5f;
	#endregion

    //Ensures dieFunction only executes once
    public bool isDead;
	// Use this for initialization
	void Start () {
		currentHealth = originalHealth;

		//Send message for script to update UI
        if (healthUpdateFunction.GetPersistentEventCount() > 0&&showHealth)
        {
            healthUpdateFunction.Invoke();
        }
		isDead = false;
		print(isDead+this.gameObject.name);

	}
    public void Reset()
    {
		print("Reset Health"+this.gameObject.name);
		Start();
    }

    // Update is called once per frame
    public void TakeDamage (float _amount) {
       
        currentHealth -= _amount;
		//Checks if the object is alive and health below 0
		if (isDead)
		{
			return;
		}
		if(currentHealth<=0){
			Die();
			return;
		}
		//if the object should regenerate set its next regen time to regen properties
		 if (regen)
        {
			//cancel previous regeneration
            CancelInvoke("Regen");
			//invoke a new regen at delay time away
            InvokeRepeating("Regen", regenDelay, regenTime);
        }
        //Send message for script to update UI
        if (healthUpdateFunction.GetPersistentEventCount() > 0&&showHealth)
        {
            healthUpdateFunction.Invoke();
        }
    }
	void Die(){
		if (isDead)
		{
			return;
		}
		isDead = true;

		print("Health Dying"+this.gameObject.name);
		//check if there is a diefunction
		if(dieFunction.GetPersistentEventCount()>0){
			//runs diefunction
			dieFunction.Invoke();
		}
		//otherwise destroy object
		else{
			Destroy(this.gameObject);
		}
		//Sets object to dead
	}
    void Regen()
    {
		//adds health
        currentHealth += regenAmount;
		//if health is maxxed out set to max and cancel regeneration
        if (currentHealth >= originalHealth)
        {
            currentHealth = originalHealth;
            CancelInvoke("Regen");
        }
		//Send message for script to update UI
        if (healthUpdateFunction.GetPersistentEventCount() > 0&&showHealth)
        {
            healthUpdateFunction.Invoke();
        }
    }
}
