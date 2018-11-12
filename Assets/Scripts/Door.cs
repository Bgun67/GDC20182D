using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
	public int doorNumber;
	public string sceneName;
	public List<GameObject> players;
	public Queue<GameObject> usingPlayers = new Queue<GameObject>();
	public bool locked;
<<<<<<< HEAD
    // Use this for initialization
    void Start()
    {
       
    }
=======
	public bool inUse;
	// Use this for initialization
	void Start()
	{
>>>>>>> 0ce790695e8c3d376003009e010bc575fc609e3b

	}

	public void OpenDoor(GameObject obj)
	{
		if (locked)
		{
			return;
		}
		usingPlayers.Enqueue(obj);
		StartCoroutine(TransitionScene());
	}


	IEnumerator TransitionScene()
	{
		yield return new WaitWhile(() => inUse);
		inUse = true;
		//Get the GameController each time
		Game_Controller _gameController = FindObjectOfType<Game_Controller>();
		//Find the player
		players = _gameController.players;
		//get the scene that the player is transitioning from
		Scene _currentScene = SceneManager.GetActiveScene();
		//make sure the scene has not been already loaded
		if (!SceneManager.GetSceneByName(sceneName).isLoaded)
		{
			if (sceneName.Length > 0)
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			}
		}
		//wait until scene has been loaded THIS CODE WORKS GREAT-- () => bool means until bool is true
		yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);
		//Find new scenes
		Scene _newScene = SceneManager.GetSceneByName(sceneName);

		//find all doors
		Door[] doors = FindObjectsOfType<Door>();
		Door _currentDoor = new Door();
		//find door with corresponding number move player to that door and forward a bit
		foreach (Door door in doors)
		{
			if (door.doorNumber == this.doorNumber)
			{
				if (door != this)
				{
					_currentDoor = door;
					break;
				}
			}
		}
		GameObject usingPlayer = usingPlayers.Dequeue();
		usingPlayer.transform.position = _currentDoor.transform.position + _currentDoor.transform.forward;
		usingPlayer.transform.forward = _currentDoor.transform.forward;
		//set the last active scene of the player
		usingPlayer.GetComponent<Player_Controller>().lastDoorNumber = _currentDoor.doorNumber;
		usingPlayer.GetComponent<Player_Controller>().currentScene = _newScene;
		usingPlayer.GetComponent<Player_Controller>().lastScene = _currentScene;

		bool destroyCurrentScene = true;
		foreach (GameObject _player in players)
		{
			Player_Controller _controller = _player.GetComponent<Player_Controller>();
			if (_controller.currentScene == _currentScene)
			{
				destroyCurrentScene = false;
			}


		}
		if (destroyCurrentScene)
		{
			print("Unload scene");
			//change scenes and unload old
			SceneManager.SetActiveScene(_newScene);
			SceneManager.UnloadSceneAsync(_currentScene);
		}
		inUse = false;


	}

}
