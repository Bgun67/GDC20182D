using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {
	public Text infoText;
    public Text healthText;
    public static UI_Manager instance;
	// Use this for initialization
	void Start () {
		//UI_Manager.instance = this;
	}
}
