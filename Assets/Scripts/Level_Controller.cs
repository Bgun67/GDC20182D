using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour {
	[System.Serializable]
	public class SpawnedEnemy
	{
		public float spawnTime;
		public GameObject enemyPrefab;
		public Transform position;
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
			StartCoroutine(Spawn(_enemy.enemyPrefab, _enemy.position, _enemy.spawnTime));
		}
	}
	IEnumerator Spawn(GameObject _enemy, Transform _position, float _time)
	{
		yield return new WaitForSeconds(_time);
		Instantiate(_enemy, _position.position, _position.rotation);
	}

}
