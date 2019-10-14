using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
	public static bool FindNearestEnemy(Vector3 _position, out GameObject foundEnemy)
	{
		foundEnemy = null;
		Collider[] _colliders = Physics.OverlapSphere(_position, 25f);
		foreach (Collider _collider in _colliders)
		{
			if (_collider.transform.root.GetComponent<Enemy>())
			{
				foundEnemy = _collider.transform.root.gameObject;
				return true;
			}
		}
		return false;

	}
}
