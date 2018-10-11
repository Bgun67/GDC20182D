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
		//destroy extra players when any scene loads
			foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player")){
				if(go.scene != SceneManager.GetActiveScene()){
					print("Destroying Extra player");
					Destroy(go);
				}
			}
	}
	
	public void OpenDoor () {
		if(locked){
			return;
		}
		//Find player
		player = GameObject.FindGameObjectWithTag("Player");
		
		//Find new scenes
		Scene _newScene = SceneManager.GetSceneByName(sceneName);
		//Check to make sure scene is loaded
		if(_newScene.isDirty){
			return;
		}
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
		//move the player to the next scene and set that scene as the current one
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
