using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item_Box : MonoBehaviour {

	public ItemName itemName;
	public GameObject itemPrefab;
	// Use this for initialization
	public void Die () {
		Instantiate(itemPrefab, transform.position, transform.rotation).GetComponent<Item>().Create(itemName);
		Destroy(this.gameObject);
	}
	
	
}
