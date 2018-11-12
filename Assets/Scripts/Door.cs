using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {
	public int doorNumber;
	public string sceneName;
	public GameObject player;
	public bool locked;
    // Use this for initialization
    void Start()
    {
       
    }

    public void OpenDoor () {
		if(locked){
			return;
		}
        StartCoroutine(TransitionScene());
    }

	
    IEnumerator TransitionScene()
    {
		//Find the player
        player = GameObject.FindGameObjectWithTag("Player");
		//get the scene that the player is transitioning from
        Scene _currentScene = SceneManager.GetActiveScene();
		//make sure the scene has not been already loaded
        if(!SceneManager.GetSceneByName(sceneName).isLoaded){
			if(sceneName.Length>0){
				SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
			
			}
		}
		//wait until scene has been loaded THIS CODE WORKS GREAT-- () => bool means until bool is true
        yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);
		//Find new scenes
		Scene _newScene = SceneManager.GetSceneByName(sceneName);
		
		//find all doors
		Door[] doors = FindObjectsOfType<Door>();
		//find door with corresponding number move player to that door and forward a bit
		foreach(Door door in doors){
			if(door.doorNumber == this.doorNumber){
				if(door != this){
					player.transform.position = door.transform.position+door.transform.forward;
					player.transform.forward = door.transform.forward;
					break;
				}
			}
		}
		//set the last active scene of the player
       // player.GetComponent<Player_Controller>().lastScene = SceneManager.GetActiveScene();
        player.GetComponent<Player_Controller>().lastDoorNumber = doorNumber;
        if (_newScene != _currentScene)
        {
            DontDestroyOnLoad(this);
            //move the player to the next scene and set that scene as the current one
            SceneManager.MoveGameObjectToScene(player, _newScene);
            //change scenes and unload old
            SceneManager.SetActiveScene(_newScene);
            SceneManager.UnloadSceneAsync(_currentScene);
            Destroy(this.gameObject);
        }

    }

}
