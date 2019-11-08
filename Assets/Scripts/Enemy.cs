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
		anim.SetFloat("Speed", 1.0f + navAgent.velocity.magnitude);
	}
	public virtual void Setup()
	{
		//get navagent
		navAgent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		InvokeRepeating("Follow", 1f, 0.1f); //retarget player every second
	}
	public virtual void Follow()
	{
		//follow the player
		target = TargetNearestPlayer();
		if (!navAgent.isOnNavMesh)
		{
			NavMeshHit _hit;
			navAgent.FindClosestEdge(out _hit);
			navAgent.Warp(_hit.position);
		}
		if (navAgent.remainingDistance > 1f||navAgent.remainingDistance == 0)
		{
			navAgent.SetDestination(target.transform.position);
			timeBetweenAttacks += 0.1f;
		}
		if (!navAgent.hasPath)
		{
			navAgent.SetDestination(target.transform.position);
		}

	}

	public virtual GameObject TargetNearestPlayer()
	{
		GameObject _closestPlayer = null;
		List<GameObject> _players =FindObjectOfType<Game_Controller>().players;
		if (_players.Count == 0)
		{
			return null;
		}
		else {
			foreach (GameObject _player in _players)
			{
                if (_player.GetComponent<Health>().isDead)
                {
                    continue; //don't target dead players
                }
				Vector3 _displacement = _player.transform.position - transform.position;
				if (_closestPlayer == null)
				{
					_closestPlayer = _player;
					continue;
				}
				if (_displacement.magnitude < (_closestPlayer.transform.position - transform.position).magnitude)
				{
					_closestPlayer = _player;
				}

			}
			return _closestPlayer;
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
            _player.transform.GetComponent<Rigidbody>().velocity = (_player.transform.position - transform.position) * 10f;
            _player.transform.GetComponent<Health>().TakeDamage(attackDamage, AttackType.Slime);
			navAgent.isStopped = true;
			timeBetweenAttacks = 0;
			Invoke("Restart", 1f);
		}
		anim.SetBool("Attack", false);
	}
	public void Recoil()
	{

		navAgent.isStopped = true;
		GetComponent<Rigidbody>().isKinematic = false;
		Invoke("Restart", 1f);
	}
	public void Restart()
	{
		navAgent.isStopped = false;
		GetComponent<Rigidbody>().isKinematic = true;
	}

}
