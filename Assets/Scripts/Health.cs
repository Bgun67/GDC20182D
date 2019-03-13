using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[Tooltip("The original health of the object")]
	public float originalHealth;
	public float currentHealth;

	[Tooltip("If left empty, object is destroyed")]
	public UnityEvent dieFunction;
	public bool showHealth;
    bool isDead;    //Ensures dieFunction only executes once

    [Tooltip("Do not use default as a critical attack type")]
	[EnumFlags] public AttackType criticalAttackType;
    [EnumFlags] public AttackType resistAttackType;
    [EnumFlags] public AttackType immuneToAttackType;

    public float criticalMultiplier;
	//public float lastUpdate;

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

	public delegate void HealthChangeHandler(float amount);
	public event HealthChangeHandler HealthChanged;

	// Use this for initialization
	void Start()
	{
		currentHealth = originalHealth;
		//Send message for script to update UI
		if (HealthChanged != null && showHealth)
		{
			HealthChanged.Invoke(0);
		}
	}
	public void Reset()
	{
		Start();
	}

    //Is there an element in A thats in B
    bool HasType(AttackType a, AttackType b)
    {
        return ((a & b) != AttackType.None);
    }

    //Is there an element in A that is not in B?
    bool HasTypeNotInB(AttackType a, AttackType b)
    {
        return (((a ^ b) & a) != AttackType.None);
    }

	// Update is called once per frame
	public void TakeDamage(float _amount, AttackType _damageType = AttackType.Normal)
	{
        if(HasTypeNotInB(_damageType,immuneToAttackType))
        {
            if (HasType(_damageType,criticalAttackType))
            {
                _amount *= criticalMultiplier;
            }
            else if (!HasTypeNotInB(_damageType,resistAttackType))
            {
                _amount /= criticalMultiplier;
            }
            currentHealth -= _amount;
        }


        if (_damageType == criticalAttackType)
		{
		}

		//Checks if the object is alive and health below 0
		if (!isDead && currentHealth <= 0)
		{
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
		if (HealthChanged != null && showHealth) HealthChanged.Invoke(-_amount);
	}

	void Die()
	{
		//check if there is a diefunction
		if (dieFunction.GetPersistentEventCount() > 0)
		{
			//runs diefunction
			dieFunction.Invoke();
		}
		//otherwise destroy object
		else
		{
			Destroy(this.gameObject);
		}
		//Sets object to dead
		isDead = true;
	}

	void Regen()
	{
		float remainingHealth = originalHealth - currentHealth;
		//adds health
		currentHealth += regenAmount;
		//if health is maxxed out set to max and cancel regeneration
		if (currentHealth >= originalHealth)
		{
			currentHealth = originalHealth;
			CancelInvoke("Regen");
		}
		//Send message for script to update UI
		if (HealthChanged != null && showHealth)
		{
			float amount;
			if (remainingHealth < regenAmount){
                amount = remainingHealth;
            }
            else{
                amount = regenAmount;
            }
			if (HealthChanged != null){
                HealthChanged.Invoke(amount);
            }
		}
	}
}
