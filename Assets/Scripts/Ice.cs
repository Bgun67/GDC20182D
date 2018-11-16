using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ice : MonoBehaviour {
	Rigidbody rb;
	NavMeshAgent agent;
	float originalSpeed;
	// Use this for initialization
	void Start () {
	
		rb = transform.root.GetComponent<Rigidbody>();
		agent = transform.root.GetComponent<NavMeshAgent>();

		if (rb != null)
		{
			rb.isKinematic = true;
		}
		if (agent != null)
		{
			originalSpeed = agent.speed;
			agent.speed = 0;
		}


	}
	
	// Update is called once per frame
	void OnDestroy () {
		if (rb != null)
		{
			rb.isKinematic = false;
		}
		if (agent != null)
		{
			agent.speed = originalSpeed;

		}
	}
}
