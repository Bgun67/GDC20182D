using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour {
	Rigidbody rb;
	// Use this for initialization
	void Start () {
	
		rb = transform.root.GetComponent<Rigidbody>();
		
		
		if (rb != null)
		{
			rb.isKinematic = true;
		}


	}
	
	// Update is called once per frame
	void OnDestroy () {
		if (rb != null)
		{
			rb.isKinematic = false;
		}
	}
}
