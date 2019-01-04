
using UnityEngine;
using UnityEditor;

public class Color_Editor : EditorWindow {
    
    string name = "";
    Color color;
    string[] dog;
    GameObject _slime;
    public class Slime_Create
    {
        public string name;
        public object type;
        public Color color;
        public Mesh mesh;
        public Transform position;
        public int amount = 1;
        
        private string get_name()
        {
            return "name";
        }

        public void SpawnSlime(GameObject _slime)
        {
            _slime.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    void colorize()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {

                renderer.sharedMaterial.color = color;

                obj.GetComponent<ObjectNames>();


            }
        }
    }

    [MenuItem("Window/Color Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<Color_Editor>("Color Editor");
    }
    // Use this for initialization
     void OnGUI()
    {
        GUILayout.Label("Pick the name and color of the mesh", EditorStyles.boldLabel);
        GUILayout.Label("Select the colour of the object");
        color = EditorGUILayout.ColorField("Colour", color);
        name = EditorGUILayout.TextField("Name of the object", name);
        //Window   
        if (GUILayout.RepeatButton("Change and Rename object"))
        {

            foreach (GameObject obj in Selection.gameObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                Shader shader = obj.GetComponent<Shader>();
                if (renderer != null)
                {
                    renderer.sharedMaterial.name = name;
                    obj.name = name;
                            
                    
                        
                }
            }
            
            Slime_Create slime = new Slime_Create();
            slime.color = color;
            slime.name = name;
            Debug.Log(slime.name);
            colorize();

        }


    }
    
    // Update is called once per frame
    
}
