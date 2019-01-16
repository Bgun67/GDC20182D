using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	NavMeshAgent navAgent;
	public GameObject target;
	public Animator anim;
	public float attackRange = 1f;
	public float attackDamage = 20f;
	[Tooltip("Time between attacking and attacking again")]
	public float attackWait = 1f;
	public float timeBetweenAttacks;
	public LayerMask layerMask;
	// Use this for initialization
	void Start () {
		Setup();
	}
	
	// Update is called once per frame
	void Update () {
	}
	public virtual void Setup()
	{
		//get navagent
		navAgent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		InvokeRepeating("Follow", 1f, 0.1f);
	}
	public virtual void Follow()
	{
		//follow the player
		print("Trying to Following");
 		target = GameObject.FindGameObjectWithTag("Player");
		if (!navAgent.isOnNavMesh)
		{
			NavMeshHit _hit;
			navAgent.FindClosestEdge(out _hit);
			navAgent.Warp(_hit.position);
		}
		if (navAgent.remainingDistance > 1f||navAgent.remainingDistance == 0)
		{
			print("Following");
			navAgent.SetDestination(target.transform.position);
			timeBetweenAttacks += 0.1f;
		}

	}
	
	void OnTriggerEnter(Collider other)
	{
		//run attack anim and run damage function
		if (other.transform.tag == "Player")
		{
			//make sure the animation is not running
			if (anim != null&&!anim.GetBool("Attack"))
			{
				anim.SetBool("Attack", true);
			}
			Attack(other.gameObject);
		}
	}
	public virtual void Attack(GameObject _player)
	{
		print("trying to attack");
		if (timeBetweenAttacks > attackWait)
		{
			_player.transform.GetComponent<Health>().TakeDamage(attackDamage, AttackType.Slime);
			_player.transform.GetComponent<Rigidbody>().velocity = (_player.transform.position - transform.position) * 10f;
			navAgent.isStopped = true;
			timeBetweenAttacks = 0;
			Invoke("Restart", 1f);
		}
		anim.SetBool("Attack", false);

 	}
	public void Restart()
	{
		navAgent.isStopped = false;
	}

}
