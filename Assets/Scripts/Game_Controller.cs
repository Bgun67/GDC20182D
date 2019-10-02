using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour {
	public string sceneName;
    public int maxNumberOfPlayers = 2;
    public int startNumberOfPlayers = 1;
	public int numberOfPlayers = 0;
	public GameObject playerPrefab;
	public GameObject camPrefab;
    public GameObject levelController;
	//[HideInInspector]
	public List<GameObject> players = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		Input_Manager.GetInputSettings();
		CheckForDuplicates();
	}

    void Update()
    {
        if (Input.GetButton("Add Player"))
        {
            AddPlayer();
        }
        else if (Input.GetButton("Remove Player")) //to make sure we can't add and remove on the same frame
        {
            RemovePlayer();
        }
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

            if (startNumberOfPlayers >= 0)
            {
                startNumberOfPlayers = 1;
            }
            if (maxNumberOfPlayers < startNumberOfPlayers)
            {
                startNumberOfPlayers = maxNumberOfPlayers;
            }
            //create the initial player(s)
            for (int i = 0; i < startNumberOfPlayers; i++)
            {
                AddPlayer();
            }
			
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
	void AddPlayer()
	{
		Level_Controller currentController = FindObjectOfType<Level_Controller>();


		if (numberOfPlayers >= maxNumberOfPlayers)
        {
            return; //TODO add text saying couldn't add new player
		}
		GameObject _newPlayer = (GameObject)Instantiate(playerPrefab);
		GetLevelController(this.sceneName).SpawnPlayer(_newPlayer);
		_newPlayer.GetComponent<Player_Controller>().playerNum = numberOfPlayers;
		DontDestroyOnLoad(_newPlayer);
		_newPlayer.GetComponent<Player_Controller>().SetupPlayer();
		_newPlayer.name = "Player " + numberOfPlayers;
        numberOfPlayers += 1;
		players.Add( _newPlayer);


        //set the camera positions for each player, the old players should be updated too
        foreach (GameObject _player in players)
        {
            _player.GetComponent<Player_Controller>().SetupPlayer();
        }
        
	}

    void RemovePlayer()
    {
        if (numberOfPlayers > 1)
        {
            //delete last player
            GameObject playerToRemove = players[numberOfPlayers - 1];
            players.Remove(playerToRemove);
            Destroy(playerToRemove.GetComponent<Camera_Follow>().mainCamera.gameObject);
            Destroy(playerToRemove);
            numberOfPlayers -= 1;

            //reset camera for all remaining players
            foreach (GameObject _player in players)
            {
                _player.GetComponent<Player_Controller>().SetupPlayer();
            }
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
