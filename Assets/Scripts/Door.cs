using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {
	public float doorNumber;
	public string sceneName;
	public GameObject player;
	public bool locked;
	// Use this for initialization
	void Start () {
	}
	
	public void OpenDoor () {
		if(locked){
			return;
		}
		player = GameObject.FindGameObjectWithTag("Player");
		Scene _currentScene = SceneManager.GetActiveScene();
		Scene _newScene = SceneManager.GetSceneByName(sceneName);
		Door[] doors = FindObjectsOfType<Door>();
		foreach(Door door in doors){
			if(door.doorNumber == this.doorNumber){
				if(door != this){
					player.transform.position = door.transform.position+door.transform.forward;
					player.transform.forward = door.transform.forward;
					break;
				}
			}
		}
		SceneManager.MoveGameObjectToScene(player, _newScene);
		SceneManager.SetActiveScene(_newScene);
	}

	public void LoadNextScene(){
		if(!SceneManager.GetSceneByName(sceneName).IsValid()){
			if(sceneName.Length>0){
				SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
			}
		}
		else{
			
		}
	}
	public void UnloadInactiveScenes(){
		foreach(Scene _scene in SceneManager.GetAllScenes()){
			if(_scene != SceneManager.GetActiveScene()){
				SceneManager.UnloadSceneAsync(_scene);
			}

		}
	}
	
}
