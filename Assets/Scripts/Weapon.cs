using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
	 [System.Serializable]
	public class Strike
	{
		public string function;
		public float damage;
		public float range;
	}
	[SerializeField]
	public Strike[] strikes;
	float currentDamage;
	float currentRange;
	[Tooltip("The location and rotation of where the holder's hand should be")]
	public Transform grip;
	//The holding gameObject of this weapon
	public GameObject root;
	//animator of the holder
	public Animator rootAnim;


	// Use this for initialization
	void OnEnable () {
		root = this.transform.root.gameObject;
		rootAnim = root.GetComponent<Animator>();
		if (rootAnim == null)
		{
			//this will pick the first available animator as backup
			rootAnim = root.GetComponentInChildren<Animator>();
		}

	}
	public void Attack(int _attackNumber)
	{
		//check to make sure the attack number (Primary, secondary) within the available strikes for this weapon
		if (_attackNumber >= strikes.Length)
		{
			return;
		}
		root.GetComponent<Player_Controller>().Invoke(strikes[_attackNumber].function,0);
		currentDamage = strikes[_attackNumber].damage;
		currentRange = strikes[_attackNumber].range;


	}
	public float GetStrikeDamage(){

		return currentDamage;

	}
	public float GetStrikeRange(){

		return currentRange;

	}




}
