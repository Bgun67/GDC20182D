using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour {
	public string sceneName;
	public int numberOfPlayers = 1;
	public GameObject playerPrefab;
	//[HideInInspector]
	public List<GameObject> players = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		CheckForDuplicates();
	}
	void CheckForDuplicates()
	{
		//Check if only gamecontroller in the scene
		if (FindObjectsOfType<Game_Controller>().Length > 1)
		{
			//check if don'tdestroy on load is on
			if (this.hideFlags != HideFlags.DontSave)
			{
				//Detect itself as virus destroy
				Destroy(this.gameObject);
			}
			else
			{
				CheckForDuplicatePlayers();
			}

		}
		else
		{
			sceneName = SceneManager.GetActiveScene().name;

			//if only one game controller set as don't destroy
			DontDestroyOnLoad(this.gameObject);
			AddPlayers();
		}
		
	}
	void CheckForDuplicatePlayers()
	{
		//destroy extra players when any scene loads
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (!players.Contains(go))
			{
				print("Destroying Extra player");
				Destroy(go);
			}

		}
	}
	void AddPlayers()
	{
		Level_Controller currentController = FindObjectOfType<Level_Controller>();
		for (int i = 0; i < numberOfPlayers; i++)
		{

			GameObject _newPlayer = (GameObject)Instantiate(playerPrefab);
			GetLevelController(this.sceneName).SpawnPlayer(_newPlayer);
			_newPlayer.GetComponent<Player_Controller>().playerNum = i;
			DontDestroyOnLoad(_newPlayer);
			_newPlayer.GetComponent<Player_Controller>().SetupPlayer();
			_newPlayer.name = "Player " + i;
			players.Add( _newPlayer);
		}

	}
	public Level_Controller GetLevelController(string _scene)
	{
		foreach (Level_Controller _controller in FindObjectsOfType<Level_Controller>())
		{
			if (_controller.scene == _scene)
			{
				return _controller;
			}
		}
		return null;
	}
	


}
