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
		}
	}
	void OnTriggerExit(Collider other){
		if(other.tag == "Player"){
			_entered = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(_entered == false){

			return;
		}
		else{
			if(inputRequired == InputType.None){
				functionToRun.Invoke();
			}
			else if(inputRequired == InputType.Key && Input.GetKey(keyCode)){
				functionToRun.Invoke();
			}
			else if(inputRequired == InputType.Button && Input.GetButton(buttonName)){
				functionToRun.Invoke();
			}
			else{
				return;
			}
		
		}
	}
}
