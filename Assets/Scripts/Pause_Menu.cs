using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause_Menu : MonoBehaviour
{   
    public static Pause_Menu _instance;
    // Start is called before the first frame update
    private void Awake()
    {
        _instance = this;
        Pause_Menu.Resume();
    }
    static void Pause()
    {
        _instance.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public static void Resume()
    {
        Time.timeScale = 1f;
        _instance.gameObject.SetActive(false);
    }
    public void QuitToMainMenu()
    {


    }
 

    // Update is called once per frame
    void Update()
    {
        
    }

}
