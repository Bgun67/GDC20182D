using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Buddy : MonoBehaviour
{
	public enum ActionState{
		Following,
		Teleport,
		Attacking,
		Idle
	}
NavMeshAgent agent;
	public Transform objToFollow;
	public float followingDistance = 5f;
	public float attackRadius = 15f;
	public ActionState currentState;
	public Attack[] attacks;
	public Animator anim;
    Game_Controller gameController;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		attacks = GetComponents<Attack>();
		anim = GetComponent<Animator>();
        gameController = FindObjectOfType<Game_Controller>();
	}
	

	// Update is called once per frame
	void Update () {
		objToFollow = gameController.players[0].transform;
		Vector3 _displacement = objToFollow.position - transform.position;
		anim.SetFloat("Run Speed", agent.speed);

		currentState = CheckState(_displacement);
		
		switch (currentState)
		{
			case ActionState.Idle:
				break;
			case ActionState.Following:
				agent.destination = FindFollowPosition();
				break;
			case ActionState.Teleport:

				Teleport(FindFollowPosition());
				break;
			case ActionState.Attacking:
				Attack();
				break;
		}
	}
	ActionState CheckState(Vector3 _displacement)
	{
		ActionState _actionState = ActionState.Idle;
		if (_displacement.magnitude > 30f)
		{
			_actionState = ActionState.Teleport;
		}
		else if (_displacement.magnitude > attackRadius)
		{
			_actionState = ActionState.Following;
		}
		else if (_displacement.magnitude < attackRadius)
		{
			
			foreach (Enemy _enemy in FindObjectsOfType<Enemy>())
			{
				if ((_enemy.transform.position - transform.position).magnitude < attackRadius)
				{
					_actionState = ActionState.Attacking;
					break;
				}
			}
			if (_actionState != ActionState.Attacking)
			{
				if (_displacement.magnitude < followingDistance)
				{
					_actionState = ActionState.Idle;
				}
				else
				{
					_actionState = ActionState.Following;
				}
			}
		}
		return _actionState;
	}
	void Attack()
	{
		//random point
		Transform _closestEnemy = null;
		foreach (Enemy _enemy in FindObjectsOfType<Enemy>())
		{
			
			Vector3 _enemyDisplacement = _enemy.transform.position-transform.position;
			if (_enemyDisplacement.magnitude > attackRadius)
			{
				continue;
			}
			if (_closestEnemy == null)
			{
				_closestEnemy = _enemy.transform;
				continue;
			}
			if (_enemyDisplacement.magnitude < (_closestEnemy.position-transform.position).magnitude)
			{
				_closestEnemy = _enemy.transform;
			}

		}

		//if close enough to the enemy
		if (_closestEnemy == null)
		{
			return;
		}
		agent.destination = _closestEnemy.position;
		if ((_closestEnemy.transform.position - transform.position).magnitude < 3f)
		{
			attacks[Random.Range(0, attacks.Length)].RunAnimation();
		}
	}
	Vector3 FindFollowPosition()
	{
		Vector3 _position  =Vector3.zero;

		//Find a following point for the player


		NavMeshHit _hit;
		for (int i = 0; i < 5f; i++)
		{
			Vector2 _randomPoint = Random.insideUnitCircle.normalized * followingDistance;
			Vector3 _testPoint = objToFollow.position + new Vector3(_randomPoint.x, 0, _randomPoint.y);
			if (NavMesh.SamplePosition(_testPoint, out _hit, 1f, NavMesh.AllAreas))
			{
				_position = _hit.position;
				break;
			}
			else
			{
				_position = transform.position;
			}
		}
		return _position;
	}
	void Teleport(Vector3 _followPosition){
		agent.Warp(_followPosition);
	}
}
