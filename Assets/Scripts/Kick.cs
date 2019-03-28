using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : Attack {

	// Use this for initialization
	void Reset () {
		actionBoolName = "Kick";
		waitBetweenActions = 2f;
		attackType = AttackType.Normal;
		power = 10f;
	}

	// Update is called once per frame
	protected override void RunAction()
	{

		print("Running Action");
		print(FindNearestEnemy());
		GiveDamage(FindNearestEnemy());
	}
	
}
