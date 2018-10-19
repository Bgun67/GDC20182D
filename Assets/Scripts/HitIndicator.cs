using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TextMeshPro))]
public class HitIndicator : MonoBehaviour
{
	TextMeshPro tmpro;
	Rigidbody rb;
	float timeAlive;
	public float timeToLive = 5;

	// Use this for initialization
	void Awake()
	{
		Debug.Log("Start on hit");
		tmpro = GetComponent<TextMeshPro>();
		rb = GetComponent<Rigidbody>();
		rb.velocity = new Vector3(Random.Range(-1, 1), 5, Random.Range(-1, 1));
		rb.angularVelocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
		Destroy(gameObject, timeToLive);
	}

	public void SetHealth(float amount)
	{
		if(amount > 0)//Heal
		{
			tmpro.faceColor = new Color32(0, 255, 0, 255);//Green
		}
		else//Hurt
		{
			tmpro.faceColor = new Color32(255, 0, 0, 255);//Green
		}
		tmpro.text = amount.ToString();
	}
}
