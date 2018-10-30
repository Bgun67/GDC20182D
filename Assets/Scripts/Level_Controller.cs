using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour {
	public Transform playerSpawnPosition;
	public string scene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SpawnPlayer(GameObject _player)
	{
		_player.transform.position = playerSpawnPosition.position;
		_player.GetComponent<Player_Controller>().isDead = false;

	}
	
}
