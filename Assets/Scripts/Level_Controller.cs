using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour {
	[System.Serializable]
	public class SpawnedEnemy
	{
		public GameObject enemyPrefab;
		public Transform position;
		public int amount = 1;
		public float initialDelay;
		public float subsequentDelay;
	}
	public SpawnedEnemy[] enemiesToSpawn;
	public Transform playerSpawnPosition;
	public string scene;
	
	// Use this for initialization
	void Start () {
		SpawnEnemies();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SpawnPlayer(GameObject _player)
	{
		_player.transform.position = playerSpawnPosition.position;

	}
	public void SpawnEnemies()
	{
		foreach (SpawnedEnemy _enemy in enemiesToSpawn)
		{
			
			StartCoroutine(Spawn(_enemy));
		}
	}
	IEnumerator Spawn(SpawnedEnemy _enemy)
	{
		//wait initial time
		yield return new WaitForSeconds(_enemy.initialDelay);
		//continue spawning enemies at delayed times until reaches amount
		for (int i = 0; i < _enemy.amount; i++)
		{
			Instantiate(_enemy.enemyPrefab, _enemy.position.position, _enemy.position.rotation);
			yield return new WaitForSeconds(_enemy.subsequentDelay);
		}
	}

}
