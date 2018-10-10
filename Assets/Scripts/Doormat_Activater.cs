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
	public bool executeOnExit;
	public string displayText;
	bool _entered = false;
	
	public InputType inputRequired;
	[Tooltip("Only use if input required is 'Key'")]
	public KeyCode keyCode;
	[Tooltip("Only use if input required is 'Button'")]

	public string buttonName = "";
	// Use this for initialization
	void Start () {
		
	}
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			_entered = true;
			if(displayText.Length>0){
				UI_Manager.instance.infoText.text = displayText;
				UI_Manager.instance.infoText.gameObject.SetActive(true);
			}
		}
	}
	void OnTriggerExit(Collider other){
		if(other.tag == "Player"){
			_entered = false;
			if(displayText.Length>0){
				UI_Manager.instance.infoText.text = "";
				UI_Manager.instance.infoText.gameObject.SetActive(false);

			}
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
