using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Input_Manager{
	static string[] inputSettings;

	public static void GetInputSettings()
	{
		try
		{
			inputSettings = System.IO.File.ReadAllLines(@"Input Settings.txt");
		}
		catch
		{
			Debug.LogError("Could not find settings file. Replacing");
			inputSettings = ResetSettings();
		}

	}
	public static float GetAxis(string _axis)
	{
		//find the index in settings array
		int _index = GetIndex(_axis);
		float _value = 0f;
		//if index was not found always return 0
		if (_index == -1)
		{
			Debug.LogError("Axis "+_axis+" Not found in input settings");
			return _value;
		}
		//if settings option is mouse x
		if (inputSettings[_index+1] == "Mouse Position X")
		{
			//normalize value
			_value = 2f*(Input.mousePosition.x/Screen.safeArea.width-0.5f);
			//if outside screen set to 0
			if (Mathf.Abs(_value) > 1f)
			{
				_value = 0;
			}
		}
		//if mouse y
		else if (inputSettings[_index+1] == "Mouse Position Y")
		{
			//normalize value
			_value = Input.mousePosition.y/Screen.safeArea.height - 0.5f;
			//set to 0
			if (Mathf.Abs(_value) > 1f)
			{
				_value = 0;
			}
		}
		else if (inputSettings[_index + 1] == "Axis")
		{
			_value = Input.GetAxis(inputSettings[_index+2]);
		}
		else
		{
			Debug.Log("No Axis Type");
			ResetSettings();
		}
		return _value;

	}
	public static float GetAxisRaw(string _axis)
	{
		int _index = GetIndex(_axis);
		float _value = 0f;
		if (_index == -1)
		{
			Debug.LogError("Axis Not found in input settings");
			return _value;
		}
		if (inputSettings[_index+1] == "Mouse Position X")
		{
			_value = 2f*(Input.mousePosition.x/Screen.safeArea.width-0.5f);
			if (Mathf.Abs(_value) > 1f)
			{
				_value = 0;
			}
		}
		else if (inputSettings[_index+1] == "Mouse Position Y")
		{
			_value = Input.mousePosition.y/Screen.safeArea.height - 0.5f;
			if (Mathf.Abs(_value) > 1f)
			{
				_value = 0;
			}
		}
		else if (inputSettings[_index + 1] == "Axis")
		{
			_value = Input.GetAxisRaw(inputSettings[_index+2]);
		}
		return _value;

	}
	public static bool GetButton(string _button)
	{
		bool _pressed = false;
		int _index = GetIndex(_button);
		if (_index == -1)
		{
			Debug.LogError("Button "+_button+" Not found in input settings");
			return _pressed;
		}
		_pressed = Input.GetButton(inputSettings[_index + 1]);
		return _pressed;
	}
	public static bool GetButtonDown(string _button)
	{
		bool _pressed = false;
		int _index = GetIndex(_button);
		if (_index == -1)
		{
			Debug.LogError("Button "+_button+" Not found in input settings");
			return _pressed;
		}
		_pressed = Input.GetButtonDown(inputSettings[_index + 1]);
		return _pressed;
	}
	//find the index within the inputsettings
	static int GetIndex(string _name)
	{
		int _index = -1;
		for (int i = 0; i < inputSettings.Length; i++)
		{
			if (inputSettings[i] == _name)
			{
				_index = i;
			}
		}
		if (_index == -1)
		{
			ResetSettings();
		}
		return _index;
	}
	static string[] ResetSettings()
	{
		string[] _settings = new string[]{
			//Axis Sort name
			"Look Vertical 1",
			//unity axis or mouse
			"Mouse Position Y",
			//name
			"Look Horizontal 1",
			"Mouse Position X",
			//name
			"Move Horizontal 1",
			//type
			"Axis",
			"Horizontal 1",
			//name
			"Move Vertical 1",
			//type
			"Axis",
			"Vertical 1",

		};

		System.IO.File.WriteAllLines(@"Input Settings.txt", _settings);
		GetInputSettings();
		return _settings;
	}
}
