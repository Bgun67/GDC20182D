using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	[System.Serializable]
	public class ItemType
	{
		public ItemName name;
		public GameObject modelPrefab;
	}
	
	public ItemType[] items;
	 ItemType currentItem;
	public Transform spawnTransform;

	void Reset()
	{
		spawnTransform = transform.Find("Spawn Transform");
	}
	public void Create(ItemName nameToSpawn)
	{
		//Add a randomized force to the item
		GetComponent<Rigidbody>().AddForce(Random.Range(0,1f),1f, Random.Range(0,1f));
		foreach (ItemType _item in items)
		{
			if (_item.name == nameToSpawn)
			{
				currentItem = _item;
				Instantiate(currentItem.modelPrefab, spawnTransform);

				return;
			}
		}
		Debug.LogError("Assign item of name" + nameToSpawn);
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<Player_Controller>().PickupItem(currentItem.name);
			Destroy(this.gameObject);
		}
	}
}
