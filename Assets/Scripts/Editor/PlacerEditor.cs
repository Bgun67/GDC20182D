using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//using UnityEditor.Events;


[CustomEditor(typeof(Random_Shitplacer))]
public class PlacerEditor : Editor {
	public bool on;
	public GameObject[] shitPrefabs;
	public int currentPrefabNumber = 0;
	bool switchPrefab;

	public GameObject currentPrefab;
	Stack<GameObject> spawnedObjects = new Stack<GameObject>();
	public float currentPrefabBounds;
	RaycastHit hit;
	bool clicked;
	float lastMousePositionX;

	public override void OnInspectorGUI(){
		DrawDefaultInspector ();
		Random_Shitplacer placer = (Random_Shitplacer) target;
		shitPrefabs = placer.shitPrefabs;
		Debug.Log ("ON gui");

	}
	void OnSceneGUI () {
		Event _currentEvent = Event.current;
		if (Physics.Raycast (HandleUtility.GUIPointToWorldRay (_currentEvent.mousePosition), out hit, 100f)) {
			
			if (switchPrefab||currentPrefab == null) {
				DestroyImmediate (currentPrefab);
				switchPrefab = false;
				Debug.Log ("Instantiating new");
				currentPrefab = GameObject.Instantiate (shitPrefabs [currentPrefabNumber], hit.point, shitPrefabs [currentPrefabNumber].transform.rotation);
				currentPrefabBounds = FindObjectBounds (currentPrefab);
			
			}
			Vector3 position = hit.point + Vector3.up * currentPrefabBounds;
			if (_currentEvent.type == EventType.ScrollWheel) {
				
			}
			if (_currentEvent.type == EventType.MouseMove) {

				lastMousePositionX =( _currentEvent.mousePosition.x-Screen.width/2f)/Screen.width;
			}

			if (_currentEvent.alt&&!clicked) {
				currentPrefab.transform.RotateAround (currentPrefab.transform.position, currentPrefab.transform.up, _currentEvent.delta.x);
			} else {
				currentPrefab.transform.position = position;
			}



			if (_currentEvent.type == EventType.KeyDown&&!clicked) {
				switch (_currentEvent.keyCode) {
				case KeyCode.A:

					currentPrefab.transform.up = hit.normal;

					break;
				case KeyCode.E:
					
					currentPrefab.transform.localScale = (Vector3.Max (Vector3.one * 0.0001f, currentPrefab.transform.lossyScale + Vector3.one * lastMousePositionX));
					Debug.Log (Vector3.one * lastMousePositionX);
					currentPrefabBounds =  FindObjectBounds (currentPrefab);
					break;
				case KeyCode.W:
					if (currentPrefabNumber < shitPrefabs.Length-1) {
						currentPrefabNumber++;
					} else {
						currentPrefabNumber = 0;
					}
					switchPrefab = true;

					break;
				case KeyCode.S:
					if (currentPrefabNumber > 0) {
						currentPrefabNumber--;
					} else {
						currentPrefabNumber = shitPrefabs.Length - 1;
					}
					switchPrefab = true;


					break;

				case KeyCode.Q:
					GameObject go = GameObject.Instantiate (currentPrefab, position, currentPrefab.transform.rotation);
					if (Event.current.keyCode == KeyCode.A) {
						go.transform.up = hit.normal;
					} else {

					}
					go.name = currentPrefab.name;
					spawnedObjects.Push (go);
					break;
				case KeyCode.D:
					if (spawnedObjects.Count > 0) {
						DestroyImmediate (spawnedObjects.Pop ());
					}
					break;

				}
			}

			if (_currentEvent.type == EventType.MouseDrag) {
				clicked = true;
				Debug.Log ("RightClick");
			} else if(_currentEvent.type == EventType.MouseUp){
				clicked = false;
			}



		}

	}
	float FindObjectBounds(GameObject _gameObject){
		float largestBound = 0;
		try{
			largestBound= _gameObject.GetComponent<MeshRenderer>().bounds.extents.y;
		}
		catch{
			
		}
		if(_gameObject.transform.childCount == 0){
			return largestBound;
		}

		MeshRenderer[] childrenContainingRenders = _gameObject.GetComponentsInChildren<MeshRenderer> ();
		for(int i = 0; i<childrenContainingRenders.Length; i++){
			float childExtent = childrenContainingRenders [i].bounds.extents.y;

			if (childExtent > largestBound) {
				largestBound = childExtent;
			}
		}
		return largestBound;
	}

}
