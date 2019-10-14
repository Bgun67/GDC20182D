using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strike : Attack {

	// Use this for initialization
	void Reset () {
		actionTriggerName = "Kick";
		waitBetweenActions = 2f;
		attackType = AttackType.Normal;
		power = 10f;
	}

	// Update is called once per frame
	protected override void RunAction()
	{
		if (used)
		{
			return;
		}
		used = true;
		print("Running Action");
		print(FindNearestEnemy());
		GiveDamage(FindNearestEnemy());
	}
	
}
