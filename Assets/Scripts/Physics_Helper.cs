using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Physics_Helper : MonoBehaviour
{
	public void MoveTo(GameObject obj, Vector3 position, float time)
	{
		StartCoroutine(MoveObject( obj,  position, time));
	}
	public IEnumerator MoveObject(GameObject obj, Vector3 position, float time)
	{

		int i = 0;
		//calculate displacement
		Vector3 _displacement = position - obj.transform.position;
		//calculate distance
		float _distance = Vector3.Magnitude(_displacement);
		float _velocity = _distance / time;
		Vector3 _displacementUV = _displacement.normalized;
		Rigidbody rb = obj.GetComponent<Rigidbody>();
		//deactivate gravity
		rb.useGravity = false;
		//make sure the cycle does not overrun
		while (i < 10000)
		{
			yield return new WaitForEndOfFrame();
			//use velocity to shift obj more
			rb.velocity = _displacementUV * _velocity;
			_displacement = position - obj.transform.position;
			if (_displacement.magnitude < 0.01f)
			{
				rb.useGravity = true;
				rb.velocity = Vector3.zero;
				break;
			}
			i++;
		}

	}
}
