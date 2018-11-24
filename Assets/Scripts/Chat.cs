using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
	[System.Serializable]
	public class Speaker
	{
		public string name;
		public Sprite image;
		public Color color;
	}
	public string txtFileName;
	[SerializeField]
	public string[,] script;
	public int line;
	public Text textField;
	public Image speakerImage;
	public Text speakerName;
	public Speaker[] speakers;

	// Use this for initialization
	public void Show()
	{
		this.gameObject.SetActive(true);


	}
	void OnEnable()
	{
		textField.text = "";
		line = 0;
		ConvertFile();

	}
	void Update()
	{
		if (Input.GetKeyDown("g"))
		{
			PlayLine();
		}
		
	}
	void ConvertFile()
	{
		string[] fileString = System.IO.File.ReadAllLines(@"Assets/Plot/" + txtFileName + ".txt");
		//replace if null
		if (fileString == null)
		{
			System.IO.File.WriteAllText(@"Assets/Plot/" + txtFileName + ".txt", "I am speechless");
			fileString = new string[] { "I am speechless" };
		}

		//create an array as the length
		script = new string[fileString.Length, 2];
		int i = 0;
		//go through file
		foreach (string line in fileString)
		{
			//divide line by name:stuff they say
			string[] lineSections = line.Split(':');
			script[i, 0] = lineSections[0];
			script[i, 1] = lineSections[1];

			i++;
		}
		PlayLine();

	}

	void PlayLine()
	{
		if (line < script.GetLength(0))
		{
			textField.text = "";
			Speaker _speaker = GetSpeaker(script[line, 0]);
			textField.text = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(_speaker.color) + ">" + script[line, 0] + ": </color></b>";
			StartCoroutine(Type(script[line, 1]));
			speakerImage.sprite = _speaker.image;
			speakerName.text = script[line, 0];
			speakerName.color = _speaker.color;
			line++;
		}
		else
		{
			this.gameObject.SetActive(false);
			this.gameObject.SetActive(true);
		}

	}
	IEnumerator Type(string _line)
	{

		string _tag = "";
		string _negativeTag = "";
		bool _inTag = false;
		int _tagStartIndex = 0;
		int _tagEndIndex = 0;
		int _startDisplacement = textField.text.Length;
		for (int i = 0; i < _line.Length; i++)
		{
			
			textField.enabled = true;
			if (_line[i] == '<')
			{
				_inTag = true;
				_tagStartIndex = i;

			}
			if (_inTag)
			{
				if (_line[i] == '>')
				{
					_tag += _line[i];
					_negativeTag = GetNegativeTag(_tag);
					_tagEndIndex = _line.IndexOf(_negativeTag);
					_line.Remove(_tagEndIndex, _negativeTag.Length);
					textField.text += _tag+_negativeTag;
				}
				else if(_negativeTag == "")
				{
					_tag += _line[i];
				}
				else
				{
					if (i >= _tagEndIndex)
					{
						i += _negativeTag.Length-1;
						_inTag = false;
						_tag = "";
						_negativeTag = "";
					}
					else
					{
						textField.text = textField.text.Insert(i+_startDisplacement, _line[i] + "");
					}
				}

				yield return null;

			}
			else
			{
				textField.text += _line[i];
				yield return new WaitForSeconds(0.01f);
			}
		}
		textField.text += "\n";
	}
	Speaker GetSpeaker(string _speakerName)
	{
		
		foreach (Speaker _speaker in speakers)
		{
			if (_speaker.name.ToLower() == _speakerName.ToLower())
			{
				return _speaker;
			}
		}
		return speakers[0];
	}
	string GetNegativeTag(string _tag)
	{
		string _negativeTag = "";
		if (_tag == "<b>")
		{
			_negativeTag = "</b>";
		}
		else if (_tag == "<i>")
		{
			_negativeTag = "</i>";
		}
		else if (_tag.StartsWith( "<color"))
		{
			_negativeTag = "</color>";
		}
		return _negativeTag;
	}
}
