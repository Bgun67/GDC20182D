using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void ThrowPlayer () {
		GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().AddForce(5000f*Vector3.up);
	}
}
