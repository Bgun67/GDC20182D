using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	public Animator anim;
	public string actionBoolName;
	public float waitBetweenActions;
	public float lastExecutionTime;
	//a multipurpose float usually used for damagepower
	public AttackType attackType;
	public float power;
	// Use this for initialization
	public void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	public void RunAnimation () {
		if (Time.time > lastExecutionTime + waitBetweenActions)
		{
			if (anim == null)
			{
				RunAction();
			}
			else
			{
				anim.SetBool(actionBoolName, true);
			}
			lastExecutionTime = Time.time;
		}
	}
	//Receives a call from the animation 
	protected virtual void RunAction()
	{
	}
	protected Enemy FindNearestEnemy()
	{
		Enemy _closestEnemy = null;
		foreach (Enemy _enemy in FindObjectsOfType<Enemy>())
		{
			Vector3 _enemyDisplacement = _enemy.transform.position-transform.position;
			if (_closestEnemy == null)
			{
				_closestEnemy = _enemy;
				continue;
			}
			if (_enemyDisplacement.magnitude < (_closestEnemy.transform.position-transform.position).magnitude)
			{
				_closestEnemy = _enemy;
			}

		}

		//if close enough to the enemy
		return _closestEnemy;
	}
	protected void GiveDamage(Enemy _enemy)
	{
		if (_enemy != null)
		{
			print("Giving Damage");
			print(power);
			_enemy.GetComponent<Health>().TakeDamage(power, attackType);
		}
	}
}
