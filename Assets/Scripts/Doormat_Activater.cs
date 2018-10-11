using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Doormat_Activater : MonoBehaviour {
	public enum InputType{
		None,
		Key,
		Button
	}
	public UnityEvent functionToRun;
	[Tooltip("Does the function execute when the player exits the trigger")]
	public bool executeOnExit;
	public string displayText;
	//Has the playe rentered the trigger
	bool _entered = false;
	[Tooltip("What does the player need to push to activate")]

	public InputType inputRequired;
	[Tooltip("Only use if input required is 'Key'")]
	public KeyCode keyCode;
	[Tooltip("Only use if input required is 'Button'")]

	public string buttonName = "";
	// Use this for initialization
	void Start () {
		
	}
	void OnTriggerEnter(Collider other){
		//if player has entered set _entered flag
		if(other.tag == "Player"){
			_entered = true;
			//if there is text display it
			if(UI_Manager.instance.infoText!=null){
				if(displayText.Length>0){
					UI_Manager.instance.infoText.text = displayText;
					UI_Manager.instance.infoText.gameObject.SetActive(true);
				}
			}
		}
	}
	void OnTriggerExit(Collider other){
		//if play has left set entered flag to false
		if(other.tag == "Player"){
			_entered = false;
			//hide text
			if(UI_Manager.instance.infoText!=null){
				UI_Manager.instance.infoText.text = "";
				UI_Manager.instance.infoText.gameObject.SetActive(false);
			}

			//run function if execute on exit
			if(executeOnExit){
				functionToRun.Invoke();

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(_entered == false||executeOnExit){

			return;
		}
		//check for input run function if fullfilled
		if(inputRequired == InputType.None){
			functionToRun.Invoke();
		}
		else if(inputRequired == InputType.Key && Input.GetKeyDown(keyCode)){
			functionToRun.Invoke();
		}
		else if(inputRequired == InputType.Button && Input.GetButtonDown(buttonName)){
			functionToRun.Invoke();
		}
		else{
			return;
		}
		
		
	}
}
